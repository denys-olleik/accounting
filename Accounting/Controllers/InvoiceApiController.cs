using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.BusinessEntityViewModels;
using Accounting.Models.InvoiceViewModels;
using Accounting.Service;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [ApiController]
  [Route("api/i")]
  public class InvoiceApiController : BaseController
  {
    private readonly JournalService _journalService;
    private readonly JournalInvoiceInvoiceLineService _journalInvoiceInvoiceLineService;
    private readonly JournalInvoiceInvoiceLinePaymentService _journalInvoiceInvoiceLinePaymentService;
    private readonly PaymentService _paymentService;
    private readonly InvoiceInvoiceLinePaymentService _invoiceInvoiceLinePaymentService;
    private readonly InvoiceLineService _invoiceLineService;
    private readonly BusinessEntityService _businessEntityService;
    private readonly RequestContext _requestContext;
    private readonly InvoiceService _invoiceService;

    public InvoiceApiController(
      JournalService journalService, 
      JournalInvoiceInvoiceLineService journalInvoiceInvoiceLineService,
      JournalInvoiceInvoiceLinePaymentService journalInvoiceInvoiceLinePaymentService,
      PaymentService paymentService,
      InvoiceInvoiceLinePaymentService invoiceInvoiceLinePaymentService,
      InvoiceLineService invoiceLineService,
      BusinessEntityService businessEntityService,
      RequestContext requestContext)
    {
      _invoiceLineService = invoiceLineService;
      _journalService = new JournalService(requestContext.DatabasePassword, requestContext.DatabaseName);
      _journalInvoiceInvoiceLineService = new JournalInvoiceInvoiceLineService(
        _invoiceLineService, 
        _journalService, 
        requestContext.DatabasePassword, 
        requestContext.DatabaseName);
      _journalInvoiceInvoiceLinePaymentService = new JournalInvoiceInvoiceLinePaymentService(requestContext.DatabasePassword, requestContext.DatabaseName);
      _paymentService = new PaymentService(requestContext.DatabasePassword, requestContext.DatabaseName);
      _invoiceInvoiceLinePaymentService = new InvoiceInvoiceLinePaymentService(requestContext.DatabasePassword, requestContext.DatabaseName);
      _businessEntityService = new BusinessEntityService(requestContext.DatabasePassword, requestContext.DatabaseName);
      _requestContext = requestContext;
      _invoiceService = new InvoiceService(_journalService, _journalInvoiceInvoiceLineService, requestContext.DatabasePassword, requestContext.DatabaseName);
    }

    [HttpGet("get-invoices")]
      public async Task<IActionResult> GetInvoices(
      int page = 1,
      int pageSize = 10,
      string inStatus = $"{Invoice.InvoiceStatusConstants.Unpaid},{Invoice.InvoiceStatusConstants.PartiallyPaid},{Invoice.InvoiceStatusConstants.Paid}",
      bool includeVoidInvoices = false)
    {
      var (invoices, nextPage) = await _invoiceService.GetAllAsync(
          page,
          pageSize,
          inStatus.Split(","),
          GetOrganizationId(),
          includeVoidInvoices);

      InvoiceInvoiceLinePaymentService invoiceInvoiceLinePaymentService 
        = new InvoiceInvoiceLinePaymentService(_requestContext.DatabaseName!, _requestContext.DatabasePassword!);
      foreach (var invoice in invoices)
      {
        invoice.Payments = await _invoiceInvoiceLinePaymentService.GetAllPaymentsByInvoiceIdAsync(invoice.InvoiceID, GetOrganizationId(), true);
        invoice.InvoiceLines = await _journalInvoiceInvoiceLineService.GetByInvoiceIdAsync(invoice.InvoiceID, GetOrganizationId(), true);

        foreach (var invoiceLine in invoice.InvoiceLines)
        {
          invoiceLine.Received = await _invoiceInvoiceLinePaymentService.GetTotalReceivedAsync(invoiceLine.InvoiceLineID, GetOrganizationId());
        }

        invoice.Received = invoice.InvoiceLines.Sum(x => x.Received);
      }

      foreach (var invoice in invoices)
      {
        invoice.BusinessEntity = await _businessEntityService.GetAsync(invoice.BusinessEntityId, GetOrganizationId());
      }

      GetInvoicesViewModel getInvoicesViewModel = new GetInvoicesViewModel
      {
        Invoices = invoices.Select(i => new InvoiceViewModel
        {
          InvoiceID = i.InvoiceID,
          RowNumber = i.RowNumber,
          InvoiceNumber = i.InvoiceNumber,
          BusinessEntity = new BusinessEntityViewModel
          {
            ID = i.BusinessEntityId,
            CustomerType = i.BusinessEntity!.CustomerType,
            FirstName = i.BusinessEntity!.FirstName,
            LastName = i.BusinessEntity!.LastName,
            CompanyName = i.BusinessEntity!.CompanyName,
          },
          Payments = i.Payments?.Select(p => new InvoiceViewModel.PaymentViewModel
          {
            PaymentID = p.PaymentID,
            Amount = p.Amount,
            VoidReason = p.VoidReason,
            ReferenceNumber = p.ReferenceNumber,
          }).ToList(),
          InvoiceLines = i.InvoiceLines?.Select(il => new InvoiceLineViewModel
          {
            ID = il.InvoiceLineID,
            Title = il.Title,
            Description = il.Description,
            Quantity = il.Quantity,
            Price = il.Price,
          }).ToList(),
          Total = i.TotalAmount,
          Received = i.Received,
          Status = i.Status,
        }).ToList(),
        Page = page,
        NextPage = nextPage,
      };

      return Ok(getInvoicesViewModel);
    }

    [HttpGet("get-invoices-filtered")]
    public async Task<IActionResult> GetInvoicesFiltered(
        string inStatus = null,
        string invoiceNumbers = null,
        string company = null)
    {
      InvoiceService invoiceService = new InvoiceService(_journalService, _journalInvoiceInvoiceLineService, _requestContext.DatabaseName!, _requestContext.DatabasePassword!);
      List<Invoice> invoices = await invoiceService.GetFilteredAsync(inStatus?.Split(","), invoiceNumbers, company, GetOrganizationId());

      foreach (var invoice in invoices)
      {
        invoice.Payments = await _invoiceInvoiceLinePaymentService.GetAllPaymentsByInvoiceIdAsync(invoice.InvoiceID, GetOrganizationId(), true);
        invoice.InvoiceLines = await _journalInvoiceInvoiceLineService.GetByInvoiceIdAsync(invoice.InvoiceID, GetOrganizationId(), true);

        foreach (var invoiceLine in invoice.InvoiceLines)
        {
          invoiceLine.Received = await _invoiceInvoiceLinePaymentService.GetTotalReceivedAsync(invoiceLine.InvoiceLineID, GetOrganizationId());
        }

        invoice.Received = invoice.InvoiceLines.Sum(x => x.Received);
      }

      foreach (var invoice in invoices)
      {
        invoice.BusinessEntity = await _businessEntityService.GetAsync(invoice.BusinessEntityId, GetOrganizationId());
      }

      GetInvoicesViewModel getInvoicesViewModel = new GetInvoicesViewModel
      {
        Invoices = invoices.Select(i => new InvoiceViewModel
        {
          InvoiceID = i.InvoiceID,
          RowNumber = i.RowNumber,
          InvoiceNumber = i.InvoiceNumber,
          BusinessEntity = new BusinessEntityViewModel
          {
            ID = i.BusinessEntity!.BusinessEntityID,
            CustomerType = i.BusinessEntity!.CustomerType,
            FirstName = i.BusinessEntity!.FirstName,
            LastName = i.BusinessEntity!.LastName,
            CompanyName = i.BusinessEntity!.CompanyName,
          },
          InvoiceLines = i.InvoiceLines?.Select(il => new InvoiceLineViewModel
          {
            ID = il.InvoiceLineID,
            Title = il.Title,
            Description = il.Description,
            Quantity = il.Quantity,
            Price = il.Price,
          }).ToList(),
          Received = i.Received,
          Status = i.Status,
        }).ToList()
      };

      return Ok(getInvoicesViewModel);
    }
  }
}