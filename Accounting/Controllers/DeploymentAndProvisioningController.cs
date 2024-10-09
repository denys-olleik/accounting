using Microsoft.AspNetCore.Mvc;
using Accounting.Models.TenantViewModels;
using Accounting.Service;
using FluentValidation.Results;
using System.Transactions;
using Accounting.Business;
using FluentValidation;
using Accounting.Models.Tenant;

namespace Accounting.Controllers
{
  [Route("dnp")]
  public class DeploymentAndProvisioningController : BaseController
  {
    private readonly TenantService _tenantService;
    private readonly CloudServices _cloudServices;
    private readonly SecretService _secretService;

    public DeploymentAndProvisioningController(TenantService tenantService, CloudServices cloudServices, SecretService secretService)
    {
      _tenantService = tenantService;
      _cloudServices = cloudServices;
      _secretService = secretService;
    }

    [Route("tenants")]
    public IActionResult Tenants()
    {
      return View();
    }

    [Route("provision-tenant")]
    [HttpGet]
    public async Task<IActionResult> ProvisionTenant()
    {
      return View();
    }

    [Route("provision-tenant")]
    [HttpPost]
    public async Task<IActionResult> ProvisionTenant(ProvisionTenantViewModel model)
    {
      model.OrganizationId = GetOrganizationId();

      ProvisionTenantViewModelValidator validator = new ProvisionTenantViewModelValidator(_tenantService, _secretService);
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        Tenant tenant;

        tenant = await _tenantService.CreateAsync(new Tenant()
        {
          Email = model.Email,
          Name = model.Name,
          CreatedById = GetUserId(),
        });

        await _cloudServices.GetDigitalOceanService(_secretService, _tenantService, GetOrganizationId()).CreateDropletAsync(tenant);

        scope.Complete();
      }

      return RedirectToAction("Tenants");
    }
  }
}

namespace Accounting.Models.TenantViewModels
{
  public class ProvisionTenantViewModel
  {
    public string? Email { get; set; }
    public string? Name { get; set; }
    public int OrganizationId { get; set; }

    public ValidationResult? ValidationResult { get; set; }
  }
}

namespace Accounting.Models.Tenant
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