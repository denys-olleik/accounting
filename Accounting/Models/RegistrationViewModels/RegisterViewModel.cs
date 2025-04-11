using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.RegistrationViewModels
{
  public class RegisterViewModel
  {
    private string? _email;
    private string? _firstName;
    private string? _lastName;
    private string? _password;
    private string? _fullyQualifiedDomainName;
    private string? _defaultNoReplyEmailAddress;
    private string? _noReplyEmailAddress;
    private string? _emailKey;
    private string? _cloudKey;
    private SecretViewModel? _dropletLimitSecret;
    private int _currentDropletCount;

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

    public string? DefaultNoReplyEmailAddress
    {
      get => _defaultNoReplyEmailAddress;
      set => _defaultNoReplyEmailAddress = value?.Trim();
    }

    public string? NoReplyEmailAddress
    {
      get => _noReplyEmailAddress;
      set => _noReplyEmailAddress = value?.Trim();
    }

    public string? EmailKey
    {
      get => _emailKey;
      set => _emailKey = value?.Trim();
    }

    public string? CloudKey
    {
      get => _cloudKey;
      set => _cloudKey = value?.Trim();
    }

    public SecretViewModel? DropletLimitSecret
    {
      get => _dropletLimitSecret;
      set => _dropletLimitSecret = value;
    }

    public int CurrentDropletCount
    {
      get => _currentDropletCount;
      set => _currentDropletCount = value;
    }

    public bool Shared { get; set; }

    public class SecretViewModel
    {
      public int? SecretID { get; set; }
      public string? Type { get; set; }
      public string? Value { get; set; }
    }

    public ValidationResult ValidationResult { get; set; } = new();

    public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
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

      public RegisterViewModelValidator()
      {
        RuleFor(x => x.Email)
          .NotEmpty()
          .EmailAddress()
          .WithMessage("Valid 'email' is required")
          .Must(DoesNotContainDisallowedCharacters)
          .WithMessage("Email contains disallowed characters.");

        RuleFor(x => x.Password)
          .NotEmpty()
          .WithMessage("'Password' is required")
          .Must(DoesNotContainDisallowedCharacters)
          .WithMessage("Password contains disallowed characters.");

        RuleFor(x => x.FirstName)
          .NotEmpty()
          .WithMessage("'First name' is required")
          .Must(DoesNotContainDisallowedCharacters)
          .WithMessage("First name contains disallowed characters.");

        RuleFor(x => x.LastName)
          .NotEmpty()
          .WithMessage("'Last name' is required")
          .Must(DoesNotContainDisallowedCharacters)
          .WithMessage("Last name contains disallowed characters.");

        RuleFor(x => x.FullyQualifiedDomainName)
          .NotEmpty()
          .When(x => !x.Shared)
          .WithMessage("'Fully qualified domain name' is required when 'Shared' is not selected.")
          .Must(DoesNotContainDisallowedCharacters)
          .WithMessage("Fully qualified domain name contains disallowed characters.");

        RuleFor(x => x)
          .Must(x => string.IsNullOrEmpty(x.CloudKey) == string.IsNullOrEmpty(x.EmailKey))
          .WithMessage("'Cloud key' and 'Email key' must both be provided or both be empty.");

        RuleFor(x => x.NoReplyEmailAddress)
          .NotEmpty()
          .When(x => !string.IsNullOrEmpty(x.EmailKey))
          .WithMessage("'No reply email address' is required when 'Email key' is provided.")
          .Must(DoesNotContainDisallowedCharacters)
          .WithMessage("No reply email address contains disallowed characters.");

        RuleFor(x => x.DefaultNoReplyEmailAddress)
          .NotEmpty()
          .When(x => string.IsNullOrEmpty(x.NoReplyEmailAddress) && string.IsNullOrEmpty(x.EmailKey))
          .WithMessage("'Default no reply email address' is required if 'No reply email address' and 'Email key' are not provided.")
          .EmailAddress()
          .WithMessage("Valid 'default no reply email address' is required.")
          .Must(DoesNotContainDisallowedCharacters)
          .WithMessage("Default no reply email address contains disallowed characters.");

        RuleFor(x => x.EmailKey)
          .Must(DoesNotContainDisallowedCharacters)
          .WithMessage("Email key contains disallowed characters.");

        RuleFor(x => x.CloudKey)
          .Must(DoesNotContainDisallowedCharacters)
          .WithMessage("Cloud key contains disallowed characters.");

        RuleFor(x => x.DropletLimitSecret)
          .NotNull()
          .When(x => !x.Shared)
          .WithMessage("Droplet limit secret must be set for non-shared instances.");

        RuleFor(x => x.DropletLimitSecret!.Value)
          .Must(value => int.TryParse(value, out _))
          .When(x => !x.Shared && x.DropletLimitSecret != null)
          .WithMessage("Invalid droplet limit value.");

        RuleFor(x => x)
          .Must(x =>
          {
            if (!x.Shared && x.DropletLimitSecret != null && int.TryParse(x.DropletLimitSecret.Value, out var limit))
            {
              return x.CurrentDropletCount < limit;
            }
            return true;
          })
          .WithMessage("Droplet limit reached. Cannot create more non-shared instances.");
      }
    }
  }
}