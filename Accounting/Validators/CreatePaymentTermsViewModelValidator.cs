using Accounting.Models.PaymentTermViewModels;
using FluentValidation;

namespace Accounting.Validators
{
    public class CreatePaymentTermsViewModelValidator : AbstractValidator<CreatePaymentTermsViewModel>
    {
        public CreatePaymentTermsViewModelValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(100).WithMessage("Description can't be longer than 100 characters.");

            RuleFor(x => x.DaysUntilDue)
                .GreaterThan(0).WithMessage("Days Until Due must be greater than 0.")
                .NotEmpty().WithMessage("Days Until Due is required.");
        }
    }
}