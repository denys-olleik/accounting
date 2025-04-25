using FluentValidation;

namespace Accounting.Models.RegistrationViewModels
{
  public class DedicatedRegistrationViewModel : BaseRegistrationViewModel
  {
    private string? _fullyQualifiedDomainName;

    public string? FullyQualifiedDomainName
    {
      get => _fullyQualifiedDomainName;
      set => _fullyQualifiedDomainName = value?.Trim();
    }

    public new FluentValidation.Results.ValidationResult? ValidationResult { get; set; } = new();

    public class DedicatedRegistrationViewModelValidator : AbstractValidator<DedicatedRegistrationViewModel>
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

      public DedicatedRegistrationViewModelValidator()
      {
        // BaseRegistrationViewModelValidator will already validate Email, FirstName, LastName, Password.
        // Only add rules for new properties:
        RuleFor(x => x.FullyQualifiedDomainName)
          .NotEmpty()
          .WithMessage("'Fully Qualified Domain Name' is required")
          .Must(DoesNotContainDisallowedCharacters)
          .WithMessage("'Fully Qualified Domain Name' contains disallowed characters.");
      }
    }
  }
}