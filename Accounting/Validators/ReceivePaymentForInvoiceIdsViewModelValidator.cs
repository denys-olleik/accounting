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

      RuleFor(x => x.ReferenceNumber)
          .NotEmpty()
          .WithMessage("'Reference number' is required.")
          .MaximumLength(100)
          .WithMessage("'Reference number' cannot exceed 100 characters.");

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
          .Must(x => ValidateTotalPaymentMatches(x))
          .WithMessage("The sum of payments does not match the total payment amount.");

      RuleForEach(x => x.Invoices)
          .Must(invoice => invoice.InvoiceLines.All(line => line.AmountToReceive > 0))
          .WithMessage("All invoice line items must have an AmountToReceive greater than 0.");
    }

    private bool ValidateTotalPaymentMatches(ReceivePaymentForInvoiceIdsViewModel viewModel)
    {
      decimal sumOfPayments = viewModel.Invoices.Sum(invoice =>
          invoice.AmountToReceive ?? invoice.InvoiceLines?.Sum(line => line.AmountToReceive ?? 0) ?? 0);

      return sumOfPayments == viewModel.PaymentTotal;
    }
  }
}