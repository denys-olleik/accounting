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
    private readonly GeneralLedgerInvoiceInvoiceLineService _generalLedgerInvoiceInvoiceLineService;

    public ReportingController(IronPdfService ironPdfService, InvoiceService invoiceService, OrganizationService organizationService, BusinessEntityService businessEntityService, GeneralLedgerInvoiceInvoiceLineService generalLedgerInvoiceInvoiceLineService)
    {
      _ironPdfService = ironPdfService;
      _invoiceService = invoiceService;
      _organizationService = organizationService;
      _businessEntityService = businessEntityService;
      _generalLedgerInvoiceInvoiceLineService = generalLedgerInvoiceInvoiceLineService;
    }

    //[HttpGet("print-invoice/{id}")]
    //public async Task<IActionResult> PrintInvoice(int id)
    //{
    //  byte[] pdf = await _ironPdfService.PrintInvoice(id, GetOrganizationId());
    //  return File(pdf, "application/pdf", $"Invoice-{id}.pdf");
    //}

    [HttpGet("view-invoice/{id}")]
    public async Task<IActionResult> ViewInvoice(int id)
    {
      Invoice invoice = await _invoiceService.GetAsync(id, GetOrganizationId());
      invoice.IssuingOrganization = await _organizationService.GetAsync(invoice.OrganizationId);
      invoice.BusinessEntity = await _businessEntityService.GetAsync(invoice.BusinessEntityId, GetOrganizationId());
      invoice.InvoiceLines = await _generalLedgerInvoiceInvoiceLineService.GetByInvoiceIdAsync(invoice.InvoiceID, GetOrganizationId(), true);

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