using FluentValidation;

namespace Accounting.Models.RegistrationViewModels
{
  public class BaseRegistrationViewModelValidator : AbstractValidator<BaseRegistrationViewModel>
  {
    protected readonly char[] _disallowedCharacters = { ';', '&', '|', '>', '<', '$', '\\', '`', '"', '\'', '/', '%', '*' };

    protected bool DoesNotContainDisallowedCharacters(string? input)
    {
      if (string.IsNullOrEmpty(input))
        return true;

      foreach (var ch in _disallowedCharacters)
      {
        if (input.Contains(ch))
        {
          return false;
        }
      }

      return true;
    }

    public BaseRegistrationViewModelValidator()
    {
      RuleFor(x => x.Email)
        .NotEmpty()
        .EmailAddress()
        .WithMessage("Valid 'email' is required")
        .Must(DoesNotContainDisallowedCharacters)
        .WithMessage("'Email' contains disallowed characters.");

      RuleFor(x => x.Password)
        .NotEmpty()
        .WithMessage("'Password' is required")
        .Must(DoesNotContainDisallowedCharacters)
        .WithMessage("'Password' contains disallowed characters.");

      RuleFor(x => x.FirstName)
        .NotEmpty()
        .WithMessage("'First name' is required")
        .Must(DoesNotContainDisallowedCharacters)
        .WithMessage("'First name' contains disallowed characters.");

      RuleFor(x => x.LastName)
        .NotEmpty()
        .WithMessage("'Last name' is required")
        .Must(DoesNotContainDisallowedCharacters)
        .WithMessage("'Last name' contains disallowed characters.");
    }
  }
}