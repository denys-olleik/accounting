using Accounting.Models.UserAccountViewModels;
using Accounting.Service;
using FluentValidation;

namespace Accounting.Validators
{
  public class LoginWithoutPasswordViewModelValidator : AbstractValidator<LoginWithoutPasswordViewModel>
  {
    private readonly LoginWithoutPasswordService _loginWithoutPasswordService;

    public LoginWithoutPasswordViewModelValidator(LoginWithoutPasswordService loginWithoutPasswordService)
    {
      _loginWithoutPasswordService = loginWithoutPasswordService;

      RuleFor(x => x.Email)
        .NotEmpty().WithMessage("'Email' is required.")
        .EmailAddress().WithMessage("Valid 'Email' is required.")
        .DependentRules(() =>
        {
          RuleFor(x => x.Code)
            .NotEmpty().WithMessage("'Code' is required.")
            .MustAsync(async (model, code, cancellation) =>
            {
              var loginWithoutPassword = await _loginWithoutPasswordService.GetAsync(model.Email!);
              return loginWithoutPassword != null &&
                     loginWithoutPassword.Code == code &&
                     loginWithoutPassword.Expires > DateTime.UtcNow;
            }).WithMessage("Invalid or expired 'Code'.");
        });
    }
  }
}