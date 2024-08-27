using Accounting.Models.SecretViewModels;
using FluentValidation;

namespace Accounting.Validators
{
  public class CreateSecretViewModelValidator : AbstractValidator<CreateSecretViewModel>
  {
    public CreateSecretViewModelValidator()
    {
      RuleFor(x => x.Key).NotEmpty().WithMessage("Name is required.");
      RuleFor(x => x.Value).NotEmpty().WithMessage("Value is required.");
    }
  }
}