using Accounting.Models.PaymentViewModels;
using FluentValidation;

namespace Accounting.Validators
{
  public class PaymentVoidValidator 
    : AbstractValidator<PaymentVoidViewModel>
  {
    public PaymentVoidValidator()
    {
      RuleFor(x => x.VoidReason)
        .NotEmpty()
        .WithMessage("Void reason is required.");
    }
  }
}