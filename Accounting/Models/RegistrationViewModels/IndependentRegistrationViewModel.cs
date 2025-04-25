using FluentValidation;

namespace Accounting.Models.RegistrationViewModels
{
  public class IndependentRegistrationViewModel : DomainRegistrationViewModel
  {
    private string? _noReplyEmailAddress;
    private string? _emailKey;
    private string? _cloudKey;

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

    public class IndependentRegistrationViewModelValidator : BaseRegistrationViewModelValidator<IndependentRegistrationViewModel>
    {
      public IndependentRegistrationViewModelValidator() : base()
      {
        RuleFor(x => x.FullyQualifiedDomainName)
          .NotEmpty()
          .WithMessage("'Fully Qualified Domain Name' is required")
          .Must(BaseRegistrationViewModel.DoesNotContainDisallowedCharacters)
          .WithMessage("'Fully Qualified Domain Name' contains disallowed characters.");

        RuleFor(x => x.NoReplyEmailAddress)
          .NotEmpty()
          .WithMessage("'No reply email address' is required")
          .Must(BaseRegistrationViewModel.DoesNotContainDisallowedCharacters)
          .WithMessage("'No reply email address' contains disallowed characters.");

        RuleFor(x => x.EmailKey)
          .NotEmpty()
          .WithMessage("'Email key' is required")
          .Must(BaseRegistrationViewModel.DoesNotContainDisallowedCharacters)
          .WithMessage("'Email key' contains disallowed characters.");

        RuleFor(x => x.CloudKey)
          .NotEmpty()
          .WithMessage("'Cloud key' is required")
          .Must(BaseRegistrationViewModel.DoesNotContainDisallowedCharacters)
          .WithMessage("'Cloud key' contains disallowed characters.");
      }
    }
  }
}