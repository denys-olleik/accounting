using Microsoft.AspNetCore.Mvc;
using Accounting.Models.TenantViewModels;

namespace Accounting.Controllers
{
  [Route("dnp")]
  public class DeploymentAndProvisioningController : BaseController
  {
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
      throw new NotImplementedException();
    }
  }
}