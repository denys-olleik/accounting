using Accounting.Common;
using Accounting.Models.AddressViewModels;
using Accounting.Models.BusinessEntityViewModels;
using FluentValidation;
using static Accounting.Business.BusinessEntity;

namespace Accounting.Validators
{
    public class EditCustomerViewModelValidator : AbstractValidator<EditBusinessEntityViewModel>
    {
        public EditCustomerViewModelValidator()
        {
            RuleFor(x => x.ID)
                .NotEmpty()
                .WithMessage("No such customer exists.");

            RuleFor(x => x.SelectedCustomerType)
                .NotEmpty()
                .WithMessage("You must select customer type.")
                .Must(BeAValidCustomerType!)
                .WithMessage("Customer type is not valid.");

            When(x => x.SelectedCustomerType == CustomerTypeConstants.Individual, () =>
            {
                RuleFor(x => x.FirstName)
                    .NotEmpty()
                    .WithMessage("First name is required for individuals.");

                RuleFor(x => x.LastName)
                    .NotEmpty()
                    .WithMessage("Last name is required for individuals.");
            });

            RuleFor(x => x.SelectedBusinessEntityTypesCsv)
                .Must(BeAValidBusinessEntityType)
                .WithMessage("You must select at least one business entity type.");

            When(x => x.SelectedCustomerType == CustomerTypeConstants.Company, () =>
            {
                RuleFor(x => x.CompanyName)
                    .NotEmpty()
                    .WithMessage("Company name is required for companies.");
            });

            RuleFor(x => x.Addresses)
                .Must(BeValidAddressList)
                .WithMessage("One or more addresses are invalid.");

            RuleFor(x => x.SelectedPaymentTermId)
            .NotEmpty()
            .WithMessage("You must select a payment term.");
        }

        private bool BeAValidBusinessEntityType(string? selectedBusinessEntityTypesCsv)
        {
            if (string.IsNullOrWhiteSpace(selectedBusinessEntityTypesCsv))
            {
                return false;
            }

            var selectedTypes = selectedBusinessEntityTypesCsv.Split(',');
            return selectedTypes.All(type => BusinessEntityTypeConstants.All.Contains(type));
        }

        private bool BeValidAddressList(List<AddressViewModel>? addresses)
        {
            if (addresses == null)
            {
                return true;
            }

            var validator = new AddressViewModelValidator();

            foreach (var address in addresses)
            {
                var result = validator.Validate(address);
                if (!result.IsValid)
                {
                    return false;
                }
            }

            return true;
        }

        private bool BeAValidCustomerType(string customerType)
        {
            return CustomerTypeConstants.All.Contains(customerType);
        }
    }
}