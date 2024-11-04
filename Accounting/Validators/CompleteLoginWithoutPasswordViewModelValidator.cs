using Accounting.Business;
using Accounting.Models.UserAccountViewModels;
using Accounting.Service;
using FluentValidation;

namespace Accounting.Validators
{
  public class CompleteLoginWithoutPasswordViewModelValidator : AbstractValidator<CompleteLoginWithoutPasswordViewModel>
  {
    private readonly LoginWithoutPasswordService _loginWithoutPasswordService;

    public CompleteLoginWithoutPasswordViewModelValidator(LoginWithoutPasswordService loginWithoutPasswordService)
    {
      _loginWithoutPasswordService = loginWithoutPasswordService;

      RuleFor(x => x.Email)
          .NotEmpty().WithMessage("'Email' is required.")
          .EmailAddress().WithMessage("Valid 'Email' is required.");

      RuleFor(x => x.Code)
          .NotEmpty().WithMessage("'Code' is required.");

      RuleFor(x => x.Email).MustAsync(HasValidLoginWithoutPassword!)
          .WithMessage("No valid login attempt found for this email.");
    }

    private async Task<bool> HasValidLoginWithoutPassword(string email, CancellationToken token)
    {
      LoginWithoutPassword? loginWithoutPassword = await _loginWithoutPasswordService.GetAsync(email);
      return loginWithoutPassword != null && !loginWithoutPassword.IsExpired;
    }
  }
}