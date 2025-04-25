using FluentValidation;

namespace Accounting.Models.RegistrationViewModels
{
  public class DedicatedRegistrationViewModel : DomainRegistrationViewModel
  {
    public class DedicatedRegistrationViewModelValidator : AbstractValidator<DedicatedRegistrationViewModel>
    {
      public DedicatedRegistrationViewModelValidator()
      {
        RuleFor(x => ((DomainRegistrationViewModel)x).FullyQualifiedDomainName)
          .NotEmpty()
          .WithMessage("'Fully Qualified Domain Name' is required")
          .Must(BaseRegistrationViewModel.DoesNotContainDisallowedCharacters)
          .WithMessage("'Fully Qualified Domain Name' contains disallowed characters.");
      }
    }
  }
}