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
      public DedicatedRegistrationViewModelValidator()
      {
        RuleFor(x => x.FullyQualifiedDomainName)
          .NotEmpty()
          .WithMessage("'Fully Qualified Domain Name' is required")
          .Must(BaseRegistrationViewModel.DoesNotContainDisallowedCharacters)
          .WithMessage("'Fully Qualified Domain Name' contains disallowed characters.");
      }
    }
  }
}