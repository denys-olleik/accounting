using Accounting.Business;
using Accounting.Models.InvoiceViewModels;
using Accounting.Service;
using FluentValidation;

namespace Accounting.Validators
{
  public class VoidInvoiceViewModelValidator : AbstractValidator<VoidInvoiceViewModel>
  {
    private readonly InvoiceService
      _invoiceService;
    private readonly InvoiceInvoiceLinePaymentService
      _invoiceInvoiceLinePaymentService;

    public VoidInvoiceViewModelValidator(
        InvoiceService invoiceService,
        int organizationId,
        InvoiceInvoiceLinePaymentService invoicePaymentService)
    {
      _invoiceService = invoiceService;
      _invoiceInvoiceLinePaymentService = invoicePaymentService;

      RuleFor(x => x.VoidReason)
        .NotEmpty()
        .WithMessage("Reason cannot be empty");

      RuleFor(x => x.InvoiceID)
        .MustAsync(async (id, cancellation) =>
        {
          bool isVoid = await _invoiceService.IsVoidAsync(id, organizationId);
          List<Payment> validPayments = await _invoiceInvoiceLinePaymentService.GetAllPaymentsByInvoiceIdAsync(id, organizationId);

          bool hasNoValidPayments = validPayments == null || validPayments.Count == 0;
          bool canBeVoided = !isVoid && hasNoValidPayments;

          return canBeVoided;
        })
        .WithMessage(x => $"Invoice '{x.InvoiceNumber}' cannot be voided because it is either void or has valid payments.");
    }
  }
}