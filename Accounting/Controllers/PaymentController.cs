using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.PaymentViewModels;
using Accounting.Service;
using Accounting.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("p")]
  public class PaymentController : BaseController
  {
    private readonly JournalService _journalService;  
    private readonly PaymentService _paymentService;
    private readonly InvoiceService _invoiceService;
    private readonly JournalInvoiceInvoiceLineService _journalInvoiceInvoiceLineService;
    private readonly InvoiceInvoiceLinePaymentService _invoiceInvoiceLinePaymentService;
    private readonly InvoiceLineService _invoiceLineService;

    public PaymentController(
      PaymentService paymentService, 
      InvoiceService invoiceService, 
      InvoiceInvoiceLinePaymentService invoiceInvoiceLinePaymentService,
      RequestContext requestContext,
      JournalService journalService,
      InvoiceLineService invoiceLineService)
    {
      _invoiceLineService = new InvoiceLineService(requestContext.DatabasePassword, requestContext.DatabaseName);
      _journalService = new JournalService(requestContext.DatabasePassword, requestContext.DatabaseName);
      _journalInvoiceInvoiceLineService = new JournalInvoiceInvoiceLineService(_invoiceLineService, _journalService, requestContext.DatabasePassword, requestContext.DatabaseName);
      _paymentService = new PaymentService(requestContext.DatabaseName);
      _invoiceService = new InvoiceService(_journalService, _journalInvoiceInvoiceLineService, requestContext.DatabasePassword, requestContext.DatabaseName);
      _invoiceInvoiceLinePaymentService = new InvoiceInvoiceLinePaymentService(requestContext.DatabasePassword, requestContext.DatabaseName);
    }

    [HttpGet]
    [Route("void/{id}")]
    public async Task<IActionResult> Void(int id)
    {
      Payment payment = await _paymentService.GetAsync(id, GetOrganizationId());

      if (payment == null)
      {
        return NotFound();
      }

      PaymentVoidViewModel model= new PaymentVoidViewModel
      {
        PaymentID = payment.PaymentID,
        ReferenceNumber = payment.ReferenceNumber,
        Amount = payment.Amount
      };

      return View(model);
    }

    [HttpPost]
    [Route("void/{id}")]
    public async Task<IActionResult> Void(PaymentVoidViewModel model)
    {
      PaymentVoidValidator validator = new PaymentVoidValidator();
      ValidationResult validationResult = await validator.ValidateAsync(model);

      Payment payment = await _paymentService.GetAsync(model.PaymentID, GetOrganizationId());
      List<Invoice> invoices = await _invoiceInvoiceLinePaymentService.GetAllInvoicesByPaymentIdAsync(model.PaymentID, GetOrganizationId());

      if (payment == null)
      {
        return NotFound();
      }

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        model.ReferenceNumber = payment.ReferenceNumber;
        return View(model);
      }

      using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        await _paymentService.VoidAsync(payment, model.VoidReason, GetUserId(), GetOrganizationId());

        foreach (Invoice invoice in invoices)
        {
          await _invoiceService.ComputeAndUpdateInvoiceStatus(invoice.InvoiceID, GetOrganizationId());
          await _invoiceService.ComputeAndUpdateTotalAmountAndReceivedAmount(invoice.InvoiceID, GetOrganizationId());
          await _invoiceService.UpdateLastUpdated(invoice.InvoiceID, GetOrganizationId());
        }

        scope.Complete();
      }

      return RedirectToAction("Invoices", "Invoice");
    }
  }
}