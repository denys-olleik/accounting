using Accounting.Business;
using Accounting.Models.SecretViewModels;
using Accounting.Service;
using Accounting.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

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
      return View();
    }

    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateSecretViewModel model)
    {
      CreateSecretViewModelValidator validator = new CreateSecretViewModelValidator();
      ValidationResult result = await validator.ValidateAsync(model);

      if (!result.IsValid)
      {
        model.ValidationResult = result;
        return View(model);
      }

      using (TransactionScope scope 
        = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        await _secretService
          .CreateAsync(model.Key, model.Value, GetOrganizationId(), GetUserId());
      };

      return RedirectToAction("Secrets");
    }
  }
}