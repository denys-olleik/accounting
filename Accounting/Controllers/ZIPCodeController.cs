using Accounting.CustomAttributes;
using Accounting.Service;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("zip")]
  public class ZIPCodeController : BaseController
  {
    private readonly ZIPCodeService _zipCodeService;

    public ZIPCodeController(ZIPCodeService zipCodeService)
    {
      _zipCodeService = zipCodeService;
    }

    public IActionResult Index()
    {
      return View();
    }
  }
}