using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  public class DeploymentAndProvisioningController : BaseController
  {
    public IActionResult Tenants()
    {
      return View();
    }
  }
}