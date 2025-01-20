using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.InvoiceViewModels;
using Accounting.Service;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("r")]
  public class ReportingController : BaseController
  {
    private readonly IronPdfService _ironPdfService;
    private readonly InvoiceService _invoiceService;
    private readonly OrganizationService _organizationService;
    private readonly BusinessEntityService _businessEntityService;
    private readonly JournalInvoiceInvoiceLineService _journalInvoiceInvoiceLineService;
    private readonly JournalService _journalService;
    private readonly InvoiceLineService _invoiceLineService;

    public ReportingController(
      IronPdfService ironPdfService, 
      InvoiceService invoiceService, 
      OrganizationService organizationService, 
      BusinessEntityService businessEntityService, 
      JournalInvoiceInvoiceLineService journalInvoiceInvoiceLineService,
      RequestContext requestContext,
      InvoiceLineService invoiceLineService)
    {
      _ironPdfService = ironPdfService;
      _invoiceLineService = new InvoiceLineService(requestContext.DatabasePassword, requestContext.DatabaseName);
      _journalService = new JournalService(requestContext.DatabasePassword, requestContext.DatabaseName);
      _invoiceService = new InvoiceService(_journalService, _journalInvoiceInvoiceLineService, requestContext.DatabasePassword, requestContext.DatabaseName);
      _organizationService = new OrganizationService(requestContext.DatabasePassword, requestContext.DatabaseName);
      _businessEntityService = new BusinessEntityService(requestContext.DatabasePassword, requestContext.DatabaseName);
      _journalInvoiceInvoiceLineService = new JournalInvoiceInvoiceLineService(_invoiceLineService, _journalService, requestContext.DatabasePassword, requestContext.DatabaseName);
    }

    [HttpGet("view-invoice/{id}")]
    public async Task<IActionResult> ViewInvoice(int id)
    {
      Invoice invoice = await _invoiceService.GetAsync(id, GetOrganizationId());
      invoice.IssuingOrganization = await _organizationService.GetAsync(invoice.OrganizationId, GetDatabaseName());
      invoice.BusinessEntity = await _businessEntityService.GetAsync(invoice.BusinessEntityId, GetOrganizationId());
      invoice.InvoiceLines = await _journalInvoiceInvoiceLineService.GetByInvoiceIdAsync(invoice.InvoiceID, GetOrganizationId(), true);

      if (IsValidJson(invoice.BillingAddressJSON!))
      {
        invoice.BillingAddress = JsonConvert.DeserializeObject<Address>(invoice.BillingAddressJSON);
      }

      if (IsValidJson(invoice.ShippingAddressJSON!))
      {
        invoice.ShippingAddress = JsonConvert.DeserializeObject<Address>(invoice.ShippingAddressJSON);
      }

      ViewInvoiceViewModel viewModel = new ViewInvoiceViewModel
      {
        InvoiceNumber = invoice.InvoiceNumber,
        DueDate = invoice.DueDate,
        OrganizationName = invoice.IssuingOrganization.Name,
        CustomerName = invoice.BusinessEntity.CompanyName,
        Address = invoice.IssuingOrganization.Address,
        AccountsReceivableEmail = invoice.IssuingOrganization.AccountsReceivableEmail,
        AccountsReceivablePhone = invoice.IssuingOrganization.AccountsReceivablePhone,
        Website = invoice.IssuingOrganization.Website,
        BillingAddress = new ViewInvoiceViewModel.AddressViewModel
        {
          ExtraAboveAddress = invoice.BillingAddress?.ExtraAboveAddress,
          AddressLine1 = invoice.BillingAddress?.AddressLine1,
          AddressLine2 = invoice.BillingAddress?.AddressLine2,
          ExtraBelowAddress = invoice.BillingAddress?.ExtraBelowAddress,
          City = invoice.BillingAddress?.City,
          StateProvince = invoice.BillingAddress?.StateProvince,
          PostalCode = invoice.BillingAddress?.PostalCode,
          Country = invoice.BillingAddress?.Country
        },
        ShippingAddress = invoice.ShippingAddress != null ? new ViewInvoiceViewModel.AddressViewModel
        {
          ExtraAboveAddress = invoice.ShippingAddress.ExtraAboveAddress,
          AddressLine1 = invoice.ShippingAddress.AddressLine1,
          AddressLine2 = invoice.ShippingAddress.AddressLine2,
          ExtraBelowAddress = invoice.ShippingAddress.ExtraBelowAddress,
          City = invoice.ShippingAddress.City,
          StateProvince = invoice.ShippingAddress.StateProvince,
          PostalCode = invoice.ShippingAddress.PostalCode,
          Country = invoice.ShippingAddress.Country
        } : null,
        InvoiceLines = invoice.InvoiceLines.Select((x, i) => new ViewInvoiceViewModel.InvoiceLineViewModel
        {
          InvoiceLineID = x.InvoiceLineID,
          RowNumber = i + 1,
          Title = x.Title,
          Description = x.Description,
          Quantity = x.Quantity,
          Price = x.Price,
          LineTotal = x.Quantity * x.Price
        }).ToList()
      };

      var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseSoftlineBreakAsHardlineBreak().Build();

      viewModel.AddressHtml = !string.IsNullOrEmpty(invoice.IssuingOrganization.Address)
        ? Markdown.ToHtml(invoice.IssuingOrganization.Address, pipeline)
        : "";

      viewModel.PaymentInstructions = !string.IsNullOrEmpty(invoice.PaymentInstructions)
        ? Markdown.ToHtml(invoice.PaymentInstructions, pipeline)
        : "";

      return View(viewModel);
    }

    //[HttpGet("print-invoice-with-attachments/{id}")]
    //public async Task<IActionResult> PrintWithAttachments(int id)
    //{
    //  byte[] pdfWithAttachments = await _ironPdfService.PrintInvoice(id, GetOrganizationId(), true);
    //  return File(pdfWithAttachments, "application/pdf", $"Invoice-w-attachments-{id}.pdf");
    //}

    bool IsValidJson(string jsonString)
    {
      if (string.IsNullOrEmpty(jsonString))
      {
        return false;
      }

      try
      {
        var jToken = JToken.Parse(jsonString);
        return true;
      }
      catch (JsonReaderException)
      {
        return false;
      }
      catch (Exception)
      {
        return false;
      }
    }
  }
}