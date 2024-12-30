using Accounting.Models.RegistrationViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [Authorize]
  [Route("registration")]
  public class RegistrationController : BaseController
  {
    [AllowAnonymous]
    [HttpGet]
    [Route("register")]
    public IActionResult Register()
    {
      return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("register")]
    public IActionResult Register(RegisterViewModel model)
    {
      throw new NotImplementedException();
    }
  }
}