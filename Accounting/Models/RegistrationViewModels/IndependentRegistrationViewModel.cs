using FluentValidation;

namespace Accounting.Models.RegistrationViewModels
{
  public class IndependentRegistrationViewModel : BaseRegistrationViewModel
  {
    public class IndependentRegistrationViewModelValidator : AbstractValidator<IndependentRegistrationViewModel>
    {
      public IndependentRegistrationViewModelValidator()
      {
        // No additional rules; inherits base rules
      }
    }
  }
}