using System.Transactions;
using Accounting.Business;
using Accounting.Common;
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
      RegisterViewModel model = new();
      model.Shared = true;

      return View(model);
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

      if (await _tenantService.ExistsAsync(model.Email!))
      {
        model.ValidationResult.Errors.Add(new ValidationFailure("Email", "Email already exists"));
        return View(model);
      }

      Tenant tenant = new()
      {
        Email = model.Email
      };

      tenant = await ProvisionDatabase(tenant);

      using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
      {
        User user = new()
        {
          Email = model.Email,
          FirstName = model.FirstName,
          LastName = model.LastName,
          Password = PasswordStorage.CreateHash(model.Password!)
        };

        UserService userService = new(tenant.DatabaseName!);

        user = await userService.CreateAsync(user);

        OrganizationService organizationService = new(tenant.DatabaseName!);

        string sampleDataPath = Path.Combine(AppContext.BaseDirectory, "sample-data-production.sql");
        string sampleDataScript = System.IO.File.ReadAllText(sampleDataPath);

        await organizationService.InsertSampleOrganizationDataAsync(sampleDataScript);

        scope.Complete();
      }

      return RedirectToAction("ThankYou", "Home");
    }

    private async Task<Tenant> ProvisionDatabase(Tenant tenant)
    {
      tenant = await _tenantService.CreateAsync(tenant);

      string createSchemaScriptPath = Path.Combine(AppContext.BaseDirectory, "create-db-script-psql.sql");
      string createSchemaScript = System.IO.File.ReadAllText(createSchemaScriptPath);

      DatabaseService databaseService = new();
      DatabaseThing database = await databaseService.CreateDatabaseAsync(tenant.PublicId);
      await databaseService.RunSQLScript(createSchemaScript, database.Name);
      await _tenantService.UpdateDatabaseName(tenant.TenantID, database.Name);

      tenant.DatabaseName = database.Name;
      return tenant;
    }
  }
}