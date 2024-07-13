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
    private readonly PaymentService _paymentService;
    private readonly InvoiceService _invoiceService;
    private readonly InvoiceInvoiceLinePaymentService _invoiceInvoiceLinePaymentService;

    public PaymentController(PaymentService paymentService, InvoiceService invoiceService, InvoiceInvoiceLinePaymentService invoiceInvoiceLinePaymentService)
    {
      _paymentService = paymentService;
      _invoiceService = invoiceService;
      _invoiceInvoiceLinePaymentService = invoiceInvoiceLinePaymentService;
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