using Accounting.Models.TenantViewModels;
using FluentValidation;
using Accounting.Service;
using Accounting.Business;

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

      RuleFor(x => x)
          .MustAsync(async (model, cancellation) =>
              await HasRequiredSecretsAsync(model.OrganizationId))
          .WithMessage("Both 'email' and 'cloud' secret keys are required to provision a tenant.");
    }

    private async Task<bool> HasRequiredSecretsAsync(int organizationId)
    {
      var emailSecret = await _secretService.GetByTypeAsync(Secret.SecretTypeConstants.Email, organizationId);
      var cloudSecret = await _secretService.GetByTypeAsync(Secret.SecretTypeConstants.Cloud, organizationId);

      return emailSecret != null && cloudSecret != null;
    }
  }
}