using Accounting.Models.InvoiceViewModels;
using FluentValidation;

namespace Accounting.Validators
{
    public class InvoiceLineViewModelValidator : AbstractValidator<InvoiceLineViewModel>
    {
        public InvoiceLineViewModelValidator()
        {
            RuleFor(x => x.ID)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.");

            RuleFor(x => x.Quantity)
                .NotEmpty()
                .WithMessage("Quantity is required.");

            RuleFor(x => x.Price)
                .NotEmpty()
                .WithMessage("Price is required.");
        }
    }
}