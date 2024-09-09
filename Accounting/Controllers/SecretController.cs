using Accounting.Business;
using Accounting.Models.SecretViewModels;
using Accounting.Service;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace Accounting.Controllers
{
  [Route("secret")]
  public class SecretController : BaseController
  {
    private readonly SecretService _secretService;

    public SecretController(SecretService secretService)
    {
      _secretService = secretService;
    }

    [Route("secrets")]
    public async Task<IActionResult> Secrets()
    {
      List<Secret> secrets = await _secretService.GetAllAsync(GetOrganizationId());

      SecretsViewModel model = new SecretsViewModel();
      model.Secrets = secrets.Select(secret => new SecretsViewModel.SecretViewModel
      {
        SecretID = secret.SecretID,
        Key = secret.Key,
        Master = secret.Master,
        Type = secret.Type,
        Purpose = secret.Purpose
      }).ToList();

      return View(model);
    }

    [Route("create")]
    [HttpGet]
    public IActionResult Create()
    {
      return View();
    }

    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateSecretViewModel model)
    {
      model.OrganizationId = GetOrganizationId();

      CreateSecretViewModelValidator validator = new CreateSecretViewModelValidator(_secretService);
      ValidationResult result = await validator.ValidateAsync(model);

      if (!result.IsValid)
      {
        model.ValidationResult = result;
        return View(model);
      }

      using (TransactionScope scope = new TransactionScope(
        TransactionScopeAsyncFlowOption.Enabled))
      {
        if (model.Master)
        {
          await _secretService.DeleteMasterAsync(GetOrganizationId());
        }

        await _secretService.CreateAsync(
          model.Key, model.Master, model.Value, model.Type, model.Purpose, GetOrganizationId(), GetUserId());

        scope.Complete();
      };

      return RedirectToAction("Secrets");
    }

    [Route("delete/{secretID}")]
    [HttpGet]
    public async Task<IActionResult> Delete(int secretID)
    {
      Secret secret = await _secretService.GetAsync(secretID, GetOrganizationId());

      DeleteSecretViewModel model = new DeleteSecretViewModel
      {
        SecretID = secret.SecretID,
        Key = secret.Key,
        Type = secret.Type,
        Purpose = secret.Purpose
      };

      return View(model);
    }

    [Route("delete/{secretID}")]
    [HttpPost]
    public async Task<IActionResult> Delete(DeleteSecretViewModel model)
    {
      await _secretService.DeleteAsync(model.SecretID, GetOrganizationId());

      return RedirectToAction("Secrets");
    }
  }
}