using FluentValidation;

namespace Accounting.Models.RegistrationViewModels
{
  public class SharedRegistrationViewModel : BaseRegistrationViewModel
  {
    public class SharedRegistrationViewModelValidator : AbstractValidator<SharedRegistrationViewModel>
    {
      public SharedRegistrationViewModelValidator()
      {

      }
    }
  }
}