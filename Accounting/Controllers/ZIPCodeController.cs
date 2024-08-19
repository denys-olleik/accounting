using Accounting.CustomAttributes;
using Accounting.Service;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("zip")]
  public class ZipCodeController : BaseController
  {
    private readonly ZipCodeService _zipCodeService;

    public ZipCodeController(ZipCodeService zipCodeService)
    {
      _zipCodeService = zipCodeService;
    }

    public IActionResult Index()
    {
      return View();
    }
  }
}