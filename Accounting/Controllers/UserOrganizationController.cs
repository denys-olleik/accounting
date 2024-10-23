using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  public class UserOrganizationController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}