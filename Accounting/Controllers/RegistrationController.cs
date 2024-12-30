using System.Transactions;
using Accounting.Business;
using Accounting.Models.RegistrationViewModels;
using Accounting.Service;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Accounting.Models.RegistrationViewModels.RegisterViewModel;

namespace Accounting.Controllers
{
  [Authorize]
  [Route("registration")]
  public class RegistrationController : BaseController
  {
    private readonly TenantService _tenantService;

    public RegistrationController(RequestContext requestContext)
    {
      _tenantService = new TenantService();
    }

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
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
      RegisterViewModelValidator validator = new();
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
      {
        Tenant tenant = new()
        {
          Email = model.Email
        };

        tenant = await _tenantService.CreateAsync(tenant);

        User user = new()
        {
          Email = model.Email
        };

        UserService userService = new(tenant.DatabaseName);

        user = await userService.CreateAsync(user);

        OrganizationService organizationService = new(tenant.DatabaseName);

        string sampleDataPath = Path.Combine(AppContext.BaseDirectory, "sample-data-production.sql");
        string sampleDataScript = System.IO.File.ReadAllText(sampleDataPath);

        await organizationService.InsertSampleOrganizationDataAsync(sampleDataScript);

        scope.Complete();
      }

      throw new NotImplementedException();
    }
  }
}