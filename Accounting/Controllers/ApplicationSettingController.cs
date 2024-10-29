using Accounting.CustomAttributes;
using Accounting.Service;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("application-settings")]
  public class ApplicationSettingController : BaseController
  {
    private readonly ApplicationSettingService _applicationSettingService;

    public ApplicationSettingController(ApplicationSettingService applicationSettingService)
    {
      _applicationSettingService = applicationSettingService;
    }

    public IActionResult Index()
    {
      return View();
    }
  }
}