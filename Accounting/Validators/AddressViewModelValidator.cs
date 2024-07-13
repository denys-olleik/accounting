using Accounting.Models.AddressViewModels;
using FluentValidation;

namespace Accounting.Validators
{
    public class AddressViewModelValidator : AbstractValidator<AddressViewModel>
    {
        public AddressViewModelValidator()
        {
            RuleFor(x => x.ID)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.AddressLine1)
                .NotEmpty()
                .WithMessage("Address Line 1 is required.");

            // No rule for AddressLine2 as it's optional

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("City is required.");

            RuleFor(x => x.StateProvince)
                .NotEmpty()
                .WithMessage("State/Province is required.");

            RuleFor(x => x.PostalCode)
                .NotEmpty()
                .WithMessage("Postal Code is required.");

            RuleFor(x => x.Country)
                .NotEmpty()
                .WithMessage("Country is required.");
        }
    }
}