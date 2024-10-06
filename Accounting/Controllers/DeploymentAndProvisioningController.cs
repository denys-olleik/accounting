using Microsoft.AspNetCore.Mvc;
using Accounting.Models.TenantViewModels;
using Accounting.Validators;
using Accounting.Service;
using FluentValidation.Results;
using System.Transactions;
using Accounting.Business;

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

        // Implement further logic as needed
        // Example: scope.Complete();
        throw new NotImplementedException();

        scope.Complete();
      }

      // Implement further logic as needed
      // Example: Redirect to success page
      throw new NotImplementedException();
    }    
  }
}