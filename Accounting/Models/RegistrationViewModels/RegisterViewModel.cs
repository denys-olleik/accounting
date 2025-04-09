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

    public bool Shared { get; set; }

    public string? FullyQualifiedDomainName
    {
      get => _fullyQualifiedDomainName;
      set => _fullyQualifiedDomainName = value?.Trim();
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

    public class SecretViewModel
    {
      public string? SecretID { get; set; }
      public string? Type { get; set; }
      public string? Value { get; set; }
    }

    public ValidationResult ValidationResult { get; set; } = new();

    public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
    {
      public RegisterViewModelValidator()
      {
        RuleFor(x => x.Email)
          .NotEmpty()
          .EmailAddress()
          .WithMessage("Valid 'email' is required");

        RuleFor(x => x.Password)
          .NotEmpty()
          .WithMessage("'Password' is required");

        RuleFor(x => x.FirstName)
          .NotEmpty()
          .WithMessage("'First name' is required");

        RuleFor(x => x.LastName)
          .NotEmpty()
          .WithMessage("'Last name' is required");

        RuleFor(x => x.FullyQualifiedDomainName)
          .NotEmpty()
          .When(x => !x.Shared)
          .WithMessage("'Fully qualified domain name' is required when 'Shared' is not selected.");

        RuleFor(x => x.EmailKey)
          .NotEmpty()
          .When(x => !string.IsNullOrEmpty(x.CloudKey))
          .WithMessage("'Email key' is required when 'Cloud key' is provided.");

        RuleFor(x => x.NoReplyEmailAddress)
          .NotEmpty()
          .When(x => !string.IsNullOrEmpty(x.EmailKey))
          .WithMessage("'No reply email address' is required when 'Email key' is provided.");

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