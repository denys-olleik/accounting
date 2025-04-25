using FluentValidation;

namespace Accounting.Models.RegistrationViewModels
{
  public class BaseRegistrationViewModelValidator<T> : AbstractValidator<T> where T : BaseRegistrationViewModel
  {
    public BaseRegistrationViewModelValidator()
    {
      RuleFor(x => x.Email)
        .NotEmpty()
        .EmailAddress()
        .WithMessage("Valid 'email' is required")
        .Must(BaseRegistrationViewModel.DoesNotContainDisallowedCharacters)
        .WithMessage("'Email' contains disallowed characters.");

      RuleFor(x => x.FirstName)
        .NotEmpty()
        .WithMessage("'First name' is required")
        .Must(BaseRegistrationViewModel.DoesNotContainDisallowedCharacters)
        .WithMessage("'First name' contains disallowed characters.");

      RuleFor(x => x.LastName)
        .NotEmpty()
        .WithMessage("'Last name' is required")
        .Must(BaseRegistrationViewModel.DoesNotContainDisallowedCharacters)
        .WithMessage("'Last name' contains disallowed characters.");

      RuleFor(x => x.Password)
        .NotEmpty()
        .WithMessage("'Password' is required")
        .Must(BaseRegistrationViewModel.DoesNotContainDisallowedCharacters)
        .WithMessage("'Password' contains disallowed characters.");
    }
  }
}