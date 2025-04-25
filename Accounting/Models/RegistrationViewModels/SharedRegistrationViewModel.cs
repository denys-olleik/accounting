using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.RegistrationViewModels
{
  public class SharedRegistrationViewModel
  {
    private string? _email;
    private string? _firstName;
    private string? _lastName;
    private string? _password;

    public string? Email
    {
      get => _email;
      set => _email = value?.Trim();
    }

    public string? FirstName
    {
      get => _firstName;
      set => _firstName = value?.Trim();
    }

    public string? LastName
    {
      get => _lastName;
      set => _lastName = value?.Trim();
    }

    public string? Password
    {
      get => _password;
      set => _password = value?.Trim();
    }

    public ValidationResult? ValidationResult { get; set; } = new();

    public class SharedRegistrationViewModelValidator : AbstractValidator<SharedRegistrationViewModel>
    {
      private readonly char[] _disallowedCharacters = { ';', '&', '|', '>', '<', '$', '\\', '`', '"', '\'', '/', '%', '*' };
      private bool DoesNotContainDisallowedCharacters(string? input)
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
      public SharedRegistrationViewModelValidator()
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
}