using Accounting.Business;
using Accounting.Models.AccountsReceivableViewModels;
using Accounting.Service;
using FluentValidation;

namespace Accounting.Validators
{
    public class VoidInvoicePaymentViewModelValidator : AbstractValidator<VoidInvoicePaymentViewModel>
    {
        private readonly InvoiceInvoiceLinePaymentService _invoicePaymentService;

        public VoidInvoicePaymentViewModelValidator(InvoiceInvoiceLinePaymentService invoicePaymentService, int organizationId)
        {
            _invoicePaymentService = invoicePaymentService;

            RuleFor(x => x.VoidReason).NotEmpty().WithMessage("Reason cannot be empty");
            RuleFor(x => x.ID)
                .MustAsync(async (id, cancellation) =>
                {
                    InvoiceInvoiceLinePayment invoicePayment = await _invoicePaymentService.GetInvoicePaymentAsync(id, organizationId);

                    return invoicePayment != null && invoicePayment.VoidReason == null;
                })
                .WithMessage(x => $"Payment cannot be voided because it does not exist or is already void.");
        }
    }
}