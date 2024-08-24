using Accounting.Models.TenantViewModels;
using FluentValidation;
using Accounting.Service;

namespace Accounting.Validators
{
  public class ProvisionTenantViewModelValidator : AbstractValidator<ProvisionTenantViewModel>
  {
    public ProvisionTenantViewModelValidator(TenantService tenantService)
    {
      RuleFor(x => x.Email)
          .NotEmpty()
          .WithMessage("Email is required.")
          .DependentRules(() =>
          {
            RuleFor(x => x.Email)
              .EmailAddress()
              .WithMessage("Invalid email format.")
              .DependentRules(() =>
                  {
                    RuleFor(x => x.Email)
                      .MustAsync(async (email, cancellation) =>
                          {
                            return !await tenantService.ExistsAsync(email!);
                          })
                      .WithMessage("A tenant with this email already exists.");
                  });
          });
    }
  }
}