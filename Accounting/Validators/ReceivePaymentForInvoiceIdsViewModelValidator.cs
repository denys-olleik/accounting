using Accounting.Models.AccountsReceivableViewModels;
using Accounting.Service;
using FluentValidation;

namespace Accounting.Validators
{
  public class ReceivePaymentForInvoiceIdsViewModelValidator : AbstractValidator<ReceivePaymentForInvoiceIdsViewModel>
  {
    private readonly InvoiceService _invoiceService;

    public ReceivePaymentForInvoiceIdsViewModelValidator(int organizationId, InvoiceService invoiceService)
    {
      _invoiceService = invoiceService;

      RuleFor(x => x.SelectedDebitAccountId)
          .NotEmpty()
          .WithMessage("You must select an account.");

      RuleFor(x => x.Invoices)
          .MustAsync(async (invoices, cancellation) =>
          {
            foreach (var invoice in invoices)
            {
              bool isVoid = await _invoiceService.IsVoidAsync(invoice.InvoiceId, organizationId);
              if (isVoid) return false;
            }
            return true;
          }).WithMessage("Payments cannot be received against void invoices.");

      RuleFor(x => x)
          .Must(x => ValidatePaymentAllocation(x.Invoices))
          .WithMessage("Each invoice must have either an itemized or a total amount to receive, but not both.");

      RuleFor(x => x)
          .Must(x => ValidateTotalPaymentMatches(x))
          .WithMessage("The sum of payments does not match the total payment amount.");
    }

    private bool ValidatePaymentAllocation(List<ReceivePaymentForInvoiceViewModel> invoices)
    {
      foreach (var invoice in invoices)
      {
        bool hasInvoiceAmountToReceive = invoice.AmountToReceive.HasValue;
        bool hasLineItemAmountsToReceive = invoice.InvoiceLines?.Any(line => line.AmountToReceive.HasValue) ?? false;

        if (hasInvoiceAmountToReceive == hasLineItemAmountsToReceive)
        {
          return false;
        }
      }
      return true;
    }

    private bool ValidateTotalPaymentMatches(ReceivePaymentForInvoiceIdsViewModel viewModel)
    {
      decimal sumOfPayments = viewModel.Invoices.Sum(invoice =>
          invoice.AmountToReceive ?? invoice.InvoiceLines?.Sum(line => line.AmountToReceive ?? 0) ?? 0);

      return sumOfPayments == viewModel.PaymentTotal;
    }
  }
}