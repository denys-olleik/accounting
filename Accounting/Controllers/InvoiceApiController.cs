using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.BusinessEntityViewModels;
using Accounting.Models.InvoiceViewModels;
using Accounting.Service;
using Microsoft.AspNetCore.Mvc;
using static Accounting.Business.Invoice;

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

    public InvoiceApiController(
      JournalService journalService, 
      JournalInvoiceInvoiceLineService journalInvoiceInvoiceLineService,
      JournalInvoiceInvoiceLinePaymentService journalInvoiceInvoiceLinePaymentService,
      PaymentService paymentService,
      InvoiceInvoiceLinePaymentService invoiceInvoiceLinePaymentService,
      InvoiceLineService invoiceLineService)
    {
      _journalService = journalService;
      _journalInvoiceInvoiceLineService = journalInvoiceInvoiceLineService;
      _journalInvoiceInvoiceLinePaymentService = journalInvoiceInvoiceLinePaymentService;
      _paymentService = paymentService;
      _invoiceInvoiceLinePaymentService = invoiceInvoiceLinePaymentService;
      _invoiceLineService = invoiceLineService;
    }

    [HttpGet("get-invoices")]
    public async Task<IActionResult> GetInvoices(
    int page = 1,
    int pageSize = 10,
    string inStatus = $"{InvoiceStatusConstants.Unpaid},{InvoiceStatusConstants.PartiallyPaid},{InvoiceStatusConstants.Paid}",
    bool includeVoidInvoices = false)
    {
      InvoiceService invoiceService = new InvoiceService(_journalService, _journalInvoiceInvoiceLineService);
      var (invoices, nextPageNumber) = await invoiceService.GetAllAsync(
          page,
          pageSize,
          inStatus.Split(","),
          GetOrganizationId(),
          includeVoidInvoices);

      InvoiceInvoiceLinePaymentService invoiceInvoiceLinePaymentService = new InvoiceInvoiceLinePaymentService();
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

      BusinessEntityService customerService = new BusinessEntityService();
      foreach (var invoice in invoices)
      {
        invoice.BusinessEntity = await customerService.GetAsync(invoice.BusinessEntityId, GetOrganizationId());
      }

      //InvoiceLineService invoiceLineService = new InvoiceLineService();
      //foreach (var invoice in invoices)
      //{
      //  invoice.InvoiceLines = await invoiceLineService.GetByInvoiceIdAsync(invoice.InvoiceID, GetOrganizationId());
      //}

      GetInvoicesViewModel getInvoicesViewModel = new GetInvoicesViewModel
      {
        Invoices = invoices.Select(i => new InvoiceViewModel
        {
          ID = i.InvoiceID,
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
        CurrentPage = page,
        NextPage = nextPageNumber,
      };

      return Ok(getInvoicesViewModel);
    }

    [HttpGet("get-invoices-filtered")]
    public async Task<IActionResult> GetInvoicesFiltered(
        string inStatus = null,
        string invoiceNumbers = null,
        string company = null)
    {
      InvoiceService invoiceService = new InvoiceService(_journalService, _journalInvoiceInvoiceLineService);
      List<Invoice> invoices = await invoiceService.SearchInvoicesAsync(inStatus?.Split(","), invoiceNumbers, company, GetOrganizationId());

      InvoiceInvoiceLinePaymentService invoicePaymentService = new InvoiceInvoiceLinePaymentService();
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

      BusinessEntityService customerService = new BusinessEntityService();
      foreach (var invoice in invoices)
      {
        invoice.BusinessEntity = await customerService.GetAsync(invoice.BusinessEntityId, GetOrganizationId());
      }

      //InvoiceLineService invoiceLineService = new InvoiceLineService();
      //foreach (var invoice in invoices)
      //{
      //  invoice.InvoiceLines = await invoiceLineService.GetByInvoiceIdAsync(invoice.InvoiceID, GetOrganizationId());
      //}

      GetInvoicesViewModel getInvoicesViewModel = new GetInvoicesViewModel
      {
        Invoices = invoices.Select(i => new InvoiceViewModel
        {
          ID = i.InvoiceID,
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