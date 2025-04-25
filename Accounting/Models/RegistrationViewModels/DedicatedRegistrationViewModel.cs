using FluentValidation;

namespace Accounting.Models.RegistrationViewModels
{
  public class DedicatedRegistrationViewModel
  {
    private string? _email;
    private string? _firstName;
    private string? _lastName;
    private string? _password;
    private string? _fullyQualifiedDomainName;

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

    public string? FullyQualifiedDomainName
    {
      get => _fullyQualifiedDomainName;
      set => _fullyQualifiedDomainName = value?.Trim();
    }

    public FluentValidation.Results.ValidationResult? ValidationResult { get; set; } = new();

    public class DedicatedRegistrationViewModelValidator : AbstractValidator<DedicatedRegistrationViewModel>
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
      public DedicatedRegistrationViewModelValidator()
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

        RuleFor(x => x.FullyQualifiedDomainName)
          .NotEmpty()
          .WithMessage("'Fully Qualified Domain Name' is required")
          .Must(DoesNotContainDisallowedCharacters)
          .WithMessage("'Fully Qualified Domain Name' contains disallowed characters.");
      }
    }
  }
}