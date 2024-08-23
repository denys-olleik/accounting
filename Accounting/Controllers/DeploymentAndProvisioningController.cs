using Microsoft.AspNetCore.Mvc;
using Accounting.Models.TenantViewModels;
using Accounting.Validators;
using Accounting.Service;
using FluentValidation.Results;

namespace Accounting.Controllers
{
  [Route("dnp")]
  public class DeploymentAndProvisioningController : BaseController
  {
    private readonly TenantService _tenantService;

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
    public async Task<IActionResult> CreateTenant(CreateTenantViewModel model)
    {
      CreateTenantViewModelValidator validator = new CreateTenantViewModelValidator(_tenantService);
      ValidationResult result = validator.Validate(model);

      throw new NotImplementedException();
    }
  }
}