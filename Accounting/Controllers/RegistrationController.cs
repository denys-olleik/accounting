using Accounting.Models.RegistrationViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [Authorize]
  [Route("registration")]
  public class RegistrationController : BaseController
  {
    [HttpGet]
    [Route("register")]
    public IActionResult Register()
    {
      return View();
    }

    [HttpPost]
    [Route("register")]
    public IActionResult Register(RegisterViewModel model)
    {
      throw new NotImplementedException();
    }
  }
}