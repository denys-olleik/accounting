using Accounting.Models.UserAccountViewModels;
using FluentValidation;

namespace Accounting.Validators
{
  public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
  {
    public LoginViewModelValidator()
    {
      RuleFor(x => x.Email)
          .NotEmpty().WithMessage("'Email' is required.")
          .EmailAddress().WithMessage("Valid 'Email' is required.");

      RuleFor(x => x.Password)
          .NotEmpty().WithMessage("'Password' is required.");
    }
  }
}