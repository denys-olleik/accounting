using Accounting.Models.Tenant;
using FluentValidation;

namespace Accounting.Validators
{
  public class TenantLoginViewModelValidator
    : AbstractValidator<TenantLoginViewModel>
  {
    public TenantLoginViewModelValidator()
    {
      RuleFor(x => x.Email)
        .NotEmpty()
        .WithMessage("Email is required.")
        .DependentRules(() =>
        {
          RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.");
        });
    }
  }
}