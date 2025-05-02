using Accounting.Business;
using Accounting.Common;
using Accounting.CustomAttributes;
using Accounting.Models.SecretViewModels;
using Accounting.Service;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace Accounting.Controllers
{
  [Authorize(Roles = ConfigurationSingleton.ConfigurationConstants.TenantManagement)]
  [Authorize(Roles = UserRoleClaimConstants.TenantManager)]
  [AuthorizeWithOrganizationId]
  [Route("secret")]
  public class SecretController : BaseController
  {
    private readonly SecretService _secretService;

    public SecretController(RequestContext requestContext, SecretService secretService)
    {
      _secretService = new SecretService(requestContext.DatabaseName, requestContext.DatabasePassword);
    }

    [Route("secrets")]
    public async Task<IActionResult> Secrets()
    {
      List<Secret> secrets = await _secretService.GetAllAsync(GetTenantId());

      SecretsViewModel model = new SecretsViewModel();
      model.Secrets = secrets.Select(secret => new SecretsViewModel.SecretViewModel
      {
        SecretID = secret.SecretID,
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
      // Secret? existingMaster = await _secretService.GetMasterAsync(GetTenantId());

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
          await _secretService.DeleteMasterAsync(GetTenantId());
        }

        Secret existingSecret = await _secretService.GetAsync(model.Type, GetTenantId());

        if (existingSecret != null)
        {
          await _secretService.DeleteAsync(existingSecret.SecretID);
        }

        await _secretService.CreateAsync(
          model.Master, model.Value, model.Type, model.Purpose, GetOrganizationId(), GetUserId(), GetTenantId());

        scope.Complete();
      };

      return RedirectToAction("Secrets");
    }

    [Route("delete/{secretID}")]
    [HttpGet]
    public async Task<IActionResult> Delete(int secretID)
    {
      Secret secret = await _secretService.GetAsync(secretID, GetTenantId());

      DeleteSecretViewModel model = new DeleteSecretViewModel
      {
        SecretID = secret.SecretID,
        Type = secret.Type,
        Purpose = secret.Purpose
      };

      return View(model);
    }

    [Route("delete/{secretID}")]
    [HttpPost]
    public async Task<IActionResult> Delete(DeleteSecretViewModel model)
    {
      await _secretService.DeleteAsync(model.SecretID);

      return RedirectToAction("Secrets");
    }
  }
}