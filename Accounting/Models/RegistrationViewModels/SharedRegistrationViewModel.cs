using FluentValidation;

namespace Accounting.Models.RegistrationViewModels
{
  public class SharedRegistrationViewModel : BaseRegistrationViewModel
  {
    public class SharedRegistrationViewModelValidator : BaseRegistrationViewModelValidator<SharedRegistrationViewModel>
    {
      public SharedRegistrationViewModelValidator() : base() { }
    }
  }
}