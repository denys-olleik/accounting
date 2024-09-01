using Accounting.Models.SecretViewModels;
using FluentValidation;

namespace Accounting.Validators
{
  public class CreateSecretViewModelValidator : AbstractValidator<CreateSecretViewModel>
  {
    public CreateSecretViewModelValidator()
    {
      RuleFor(x => x.Key).NotEmpty().WithMessage("Key is required.")
                         .MaximumLength(100).WithMessage("Key cannot exceed 100 characters.");
      RuleFor(x => x.Value).NotEmpty().WithMessage("Value is required.");
      RuleFor(x => x.Vendor).MaximumLength(20).WithMessage("Vendor name cannot exceed 20 characters.");
      RuleFor(x => x.Purpose).MaximumLength(100).WithMessage("Purpose cannot exceed 100 characters.");
    }
  }
}