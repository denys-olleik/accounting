using Accounting.Models.TenantViewModels;
using FluentValidation;
using Accounting.Service;

namespace Accounting.Validators
{
  public class ProvisionTenantViewModelValidator : AbstractValidator<ProvisionTenantViewModel>
  {
    private readonly TenantService _tenantService;
    private readonly SecretService _secretService;

    public ProvisionTenantViewModelValidator(TenantService tenantService, SecretService secretService)
    {
      _tenantService = tenantService;
      _secretService = secretService;

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
                            return !await _tenantService.ExistsAsync(email!);
                          })
                      .WithMessage("A tenant with this email already exists.");
                  });
          });
    }
  }
}