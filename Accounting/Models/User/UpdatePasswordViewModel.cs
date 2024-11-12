using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.User
{
  public class UpdatePasswordViewModel
  {
    private string _newPassword;
    private string _confirmPassword;

    public string NewPassword
    {
      get => _newPassword;
      set => _newPassword = value?.Trim();
    }

    public string ConfirmPassword
    {
      get => _confirmPassword;
      set => _confirmPassword = value?.Trim();
    }

    public ValidationResult ValidationResult { get; set; }

    public class UpdatePasswordViewModelValidator : AbstractValidator<UpdatePasswordViewModel>
    {
      public UpdatePasswordViewModelValidator()
      {
        RuleFor(x => x.NewPassword)
          .NotEmpty().WithMessage("Password is required.")
          .DependentRules(() =>
          {
            RuleFor(x => x.ConfirmPassword)
              .NotEmpty().WithMessage("Password is required.")
              .DependentRules(() =>
              {
                RuleFor(x => x.ConfirmPassword)
                  .Equal(x => x.NewPassword).WithMessage("Passwords must match.");
              });
          });
      }
    }
  }
}