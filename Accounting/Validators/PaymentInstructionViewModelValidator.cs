using Accounting.Models.PaymentInstructionViewModels;
using FluentValidation;

namespace Accounting.Validators
{
    public class PaymentInstructionViewModelValidator 
        : AbstractValidator<CreatePaymentInstructionViewModel>
    {
        public PaymentInstructionViewModelValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.");

            RuleFor(x => x.Content)
                .NotEmpty()
                .WithMessage("Content is required.");
        }
    }
}