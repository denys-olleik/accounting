using Accounting.Common;
using Accounting.CustomAttributes;
using Accounting.Models.BusinessEntityViewModels;
using Accounting.Models.InvoicePaymentViewModels;
using Accounting.Models.InvoiceViewModels;
using Accounting.Models.PaymentViewModels;
using Accounting.Service;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [ApiController]
  [Route("api/ip")]
  public class InvoicePaymentApiController : BaseController
  {
    private readonly InvoiceInvoiceLinePaymentService _invoicePaymentService;
    private readonly BusinessEntityService _businessEntityService;

    public InvoicePaymentApiController(
      InvoiceInvoiceLinePaymentService invoicePaymentService,
      InvoiceService invoiceService,
      BusinessEntityService businessEntityService)
    {
      _invoicePaymentService = invoicePaymentService;
      _businessEntityService = businessEntityService;
    }

    [HttpGet("get-invoice-payments")]
    public async Task<IActionResult> GetInvoicePayments(
      int page,
      int pageSize)
    {
      var (invoicePayments, nextPageNumber) = await _invoicePaymentService
        .GetAllAsync(
          page,
          pageSize,
          GetOrganizationId(),
          new List<string>() {
            TypesToLoadConstants.Invoice,
            TypesToLoadConstants.Payment
          });

      foreach (var invoicePayment in invoicePayments)
      {
        invoicePayment.Invoice!.BusinessEntity
          = await _businessEntityService.GetAsync(
            invoicePayment.Invoice.BusinessEntityId,
            GetOrganizationId());
      }

      GetInvoicePaymentsApiViewModel getInvoicePaymentsViewModel = new GetInvoicePaymentsApiViewModel
      {
        InvoicePayments = invoicePayments.Select(x => new InvoicePaymentViewModel
        {
          ID = x.InvoiceInvoiceLinePaymentID,
          RowNumber = x.RowNumber,
          Invoice = new InvoiceViewModel
          {
            InvoiceID = x.Invoice!.InvoiceID,
            InvoiceNumber = x.Invoice.InvoiceNumber,
            BusinessEntity = new BusinessEntityViewModel
            {
              ID = x.Invoice.BusinessEntity!.BusinessEntityID,
              FirstName = x.Invoice.BusinessEntity.FirstName,
              LastName = x.Invoice.BusinessEntity.LastName,
              CompanyName = x.Invoice.BusinessEntity.CompanyName,
              CustomerType = x.Invoice.BusinessEntity.CustomerType,
            },
          },
          Payment = new PaymentViewModel
          {
            ID = x.Payment!.PaymentID,
            ReferenceNumber = x.Payment.ReferenceNumber,
          },
          Amount = x.Amount,
          Created = x.Created,
        }).ToList(),
        Page = page,
        NextPage = nextPageNumber,
      };

      return Ok(getInvoicePaymentsViewModel);
    }

    [HttpGet("search-invoice-payments")]
    public async Task<IActionResult> SearchInvoicePayments(
      string customerSearchQuery,
      int page = 1,
      int pageSize = 10)
    {
      var (invoicePayments, nextPageNumber) = await _invoicePaymentService.SearchInvoicePaymentsAsync(
        page,
        pageSize,
        customerSearchQuery,
        new List<string>() { TypesToLoadConstants.Invoice, TypesToLoadConstants.Payment },
        GetOrganizationId());

      foreach (var invoicePayment in invoicePayments)
      {
        invoicePayment.Invoice!.BusinessEntity
          = await _businessEntityService.GetAsync(
            invoicePayment.Invoice.BusinessEntityId,
            GetOrganizationId());
      }

      SearchInvoicePaymentsApiViewModel searchInvoicePaymentsViewModel = new SearchInvoicePaymentsApiViewModel
      {
        InvoicePayments = invoicePayments.Select(x => new InvoicePaymentViewModel
        {
          ID = x.InvoiceInvoiceLinePaymentID,
          RowNumber = x.RowNumber,
          Invoice = new InvoiceViewModel
          {
            InvoiceID = x.Invoice!.InvoiceID,
            InvoiceNumber = x.Invoice.InvoiceNumber,
            BusinessEntity = new BusinessEntityViewModel
            {
              ID = x.Invoice.BusinessEntity!.BusinessEntityID,
              FirstName = x.Invoice.BusinessEntity.FirstName,
              LastName = x.Invoice.BusinessEntity.LastName,
              CompanyName = x.Invoice.BusinessEntity.CompanyName,
              CustomerType = x.Invoice.BusinessEntity.CustomerType,
            },
          },
          Payment = new PaymentViewModel
          {
            ID = x.Payment!.PaymentID,
            ReferenceNumber = x.Payment.ReferenceNumber,
          },
          Amount = x.Amount,
          Created = x.Created,
        }).ToList(),
        Page = page,
        NextPage = nextPageNumber,
        PageSize = pageSize,
      };

      return Ok(searchInvoicePaymentsViewModel);
    }
  }
}