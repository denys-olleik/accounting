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
      model.Secrets = secrets.Select(s => new SecretViewModel
      {
        SecretID = s.SecretID,
        Key = s.Key,
        Vendor = s.Vendor,
        Purpose = s.Purpose
      }).ToList();

      return View(model);
    }
  }
}