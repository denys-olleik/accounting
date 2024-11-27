using Accounting.Business;
using Accounting.Service;
using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.TenantViewModels
{
  public class ProvisionTenantViewModel
  {
    public string? Email { get; set; }
    public bool Shared { get; set; }
    public string? FullyQualifiedDomainName { get; set; }
    public int OrganizationId { get; set; }

    public ValidationResult? ValidationResult { get; set; }

    public class ProvisionTenantViewModelValidator
     : AbstractValidator<ProvisionTenantViewModel>
    {
      private readonly TenantService _tenantService;
      private readonly SecretService _secretService;

      public ProvisionTenantViewModelValidator(
          TenantService tenantService,
          SecretService secretService)
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
                RuleFor(x => x)
                  .MustAsync(async (model, cancellation) =>
                  {
                    return !await _tenantService.ExistsAsync(model.Email!);
                  })
                  .WithMessage("A tenant with this email already exists.");
              });
          });

        RuleFor(x => x)
          .MustAsync(async (model, cancellation) =>
              await HasRequiredSecretsAsync(model.OrganizationId, model.Shared))
          .WithMessage("The required secret keys are not available for provisioning a tenant.");

        RuleFor(x => x.FullyQualifiedDomainName)
          .NotEmpty()
          .When(x => !x.Shared)
          .WithMessage("'Fully Qualified Domain Name' is required when 'Shared' is not selected.");
      }

      private async Task<bool> HasRequiredSecretsAsync(int organizationId, bool isShared)
      {
        var emailSecret = await _secretService.GetAsync(
            Secret.SecretTypeConstants.Email,
            organizationId);

        if (isShared)
        {
          return emailSecret != null;
        }
        else
        {
          var cloudSecret = await _secretService.GetAsync(
              Secret.SecretTypeConstants.Cloud,
              organizationId);
          return emailSecret != null && cloudSecret != null;
        }
      }
    }
  }
}