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

    public DeploymentAndProvisioningController(TenantService tenantService)
    {
      _tenantService = tenantService;
    }

    [Route("tenants")]
    public IActionResult Tenants()
    {
      return View();
    }

    [Route("create-tenant")]
    [HttpGet]
    public async Task<IActionResult> CreateTenant()
    {
      return View();
    }

    [Route("create-tenant")]
    [HttpPost]
    public async Task<IActionResult> ProvisionTenant(ProvisionTenantViewModel model)
    {
      ProvisionTenantViewModelValidator validator = new ProvisionTenantViewModelValidator(_tenantService);
      ValidationResult validationResult = validator.Validate(model);

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
          CreatedById = GetUserId(),
        });

        await _cloudServices.GetDigitalOceanService().CreateDropletAsync(tenant);

        throw new NotImplementedException();

        scope.Complete();
      }

      throw new NotImplementedException();
    }
  }
}