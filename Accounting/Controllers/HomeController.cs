using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  public class HomeController : BaseController
  {
    public IActionResult Index()
    {
      return View();
    }

    [Route("careers")]
    public IActionResult Careers()
    {
      return View();
    }

    
  }
}