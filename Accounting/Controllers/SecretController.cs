using Accounting.Business;
using Accounting.Models.SecretViewModels;
using Accounting.Service;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [Route("secrets")]
  public class SecretController : BaseController
  {
    private readonly SecretService _secretService;

    public SecretController(SecretService secretService)
    {
      _secretService = secretService;
    }

    public async Task<IActionResult> Secrets()
    {
      List<Secret> secrets = await _secretService.GetAllAsync(GetOrganizationId());

      SecretsViewModel model = new SecretsViewModel();
      model.Secrets = new List<SecretViewModel>();

      return View(model);
    }

    [Route("create")]
    [HttpGet]
    public IActionResult Create()
    {
      throw new NotImplementedException();
      return View();
    }

    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateSecretViewModel model)
    {
      throw new NotImplementedException();

      return RedirectToAction("Secrets");
    }
  }
}