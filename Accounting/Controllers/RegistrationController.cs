using System.Transactions;
using Accounting.Business;
using Accounting.Common;
using Accounting.Models.RegistrationViewModels;
using Accounting.Service;
using DigitalOcean.API.Exceptions;
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
    private readonly DatabaseService _databaseService = new();
    private readonly SecretService _secretService;
    private readonly UserService _userService;
    private readonly UserOrganizationService _userOrganizationService = new();

    public RegistrationController(
      RequestContext requestContext,
      DatabaseService databaseService,
      UserService userService,
      UserOrganizationService userOrganizationService)
    {
      _tenantService = new TenantService();
      _databaseService = databaseService;
      _secretService = new SecretService();
      _userService = new UserService();
      _userOrganizationService = new UserOrganizationService();
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
      Tenant defaultTenant = await _tenantService.GetByDatabaseNameAsync(DatabaseThing.DatabaseConstants.DatabaseName);
      Secret defaultNoReplyEmailSecret = await _secretService.GetAsync(Secret.SecretTypeConstants.NoReply, defaultTenant.TenantID);

      model.DefaultNoReplyEmailAddress = defaultNoReplyEmailSecret?.Value;

      RegisterViewModelValidator validator = new();
      if (!model.Shared)
      {
        var dropletLimitSecret = await _secretService.GetAsync(Secret.SecretTypeConstants.DropletLimit, defaultTenant.TenantID);
        model.DropletLimitSecret = dropletLimitSecret != null ? new SecretViewModel()
        {
          SecretID = dropletLimitSecret.SecretID,
          Type = dropletLimitSecret.Type,
          Value = dropletLimitSecret.Value
        } : null;

        model.CurrentDropletCount = await _tenantService.GetCurrentDropletCountAsync();
      }

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

      Tenant tenant;

      if (model.Shared)
      {
        using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
        {
          tenant = await _tenantService.CreateAsync(new Tenant()
          {
            Email = model.Email,
            DatabasePassword = defaultTenant.DatabasePassword
          });

          scope.Complete();
        }

        string createSchemaScriptPath = Path.Combine(AppContext.BaseDirectory, "create-db-script-psql.sql");
        string createSchemaScript = System.IO.File.ReadAllText(createSchemaScriptPath);

        DatabaseThing database = await _databaseService.CreateDatabaseAsync(tenant.PublicId);
        await _databaseService.RunSQLScript(createSchemaScript, database.Name);
        await _tenantService.UpdateDatabaseName(tenant.TenantID, database.Name);

        using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
        {
          User user = new()
          {
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Password = PasswordStorage.CreateHash(model.Password!)
          };

          UserService userService = new UserService(database.Name!, tenant.DatabasePassword!);
          user = await userService.CreateAsync(user);

          OrganizationService organizationService = new(database.Name!, tenant.DatabasePassword);

          string sampleDataPath = Path.Combine(AppContext.BaseDirectory, "sample-data-production.sql");
          string sampleDataScript = System.IO.File.ReadAllText(sampleDataPath);

          await organizationService.InsertSampleOrganizationDataAsync(sampleDataScript);

          UserOrganizationService userOrganizationService = new();
          await userOrganizationService.CreateAsync(user.UserID, 1, database.Name!, tenant.DatabasePassword);

          scope.Complete();
        }
      }
      else
      {
        Secret? cloudSecret = null;
        string? emailSecretValue = null;

        if (string.IsNullOrEmpty(model.CloudKey))
        {
          cloudSecret = await _secretService.GetAsync(Secret.SecretTypeConstants.Cloud, defaultTenant.TenantID);
          if (cloudSecret == null)
          {
            model.ValidationResult.Errors.Add(new ValidationFailure("Shared", "Cloud secret not found."));
            return View(model);
          }
        }

        if (!string.IsNullOrWhiteSpace(model.EmailKey))
        {
          emailSecretValue = model.EmailKey;
        }
        else
        {
          emailSecretValue = await GetEmailSecretAsync(defaultTenant.TenantID);
          if (string.IsNullOrEmpty(emailSecretValue))
          {
            model.ValidationResult.Errors.Add(new ValidationFailure("EmailKey", "Email secret not found or invalid."));
            return View(model);
          }
        }

        using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
        {
          tenant = await _tenantService.CreateAsync(new Tenant()
          {
            Email = model.Email,
            FullyQualifiedDomainName = model.FullyQualifiedDomainName,
            DatabasePassword = RandomHelper.GenerateSecureAlphanumericString(20),
          });

          var cloudServices = new CloudServices(_secretService, _tenantService);

          try
          {
            await cloudServices.GetDigitalOceanService().CreateDropletAsync(
              tenant,
              tenant.DatabasePassword, tenant.Email, model.Password, null!, null!, false,
              emailSecretValue, model.FullyQualifiedDomainName,
              string.IsNullOrEmpty(model.CloudKey) ? null : model.CloudKey, model.NoReplyEmailAddress ?? defaultNoReplyEmailSecret?.Value!);
          }
          catch (ApiException e)
          {
            if (e.Message != "Access Denied")
            {
              throw;
            }

            model.ValidationResult.Errors.Add(new ValidationFailure("Shared", "Access denied"));
            return View(model);
          }

          scope.Complete();
        }
      }

      return RedirectToAction("RegistrationComplete", "Registration");
    }

    private async Task<string?> GetEmailSecretAsync(int defaultTenantId)
    {
      Secret? emailSecret = await _secretService.GetAsync(Secret.SecretTypeConstants.Email, defaultTenantId);
      return emailSecret?.Value;
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("registration-complete")]
    public IActionResult RegistrationComplete()
    {
      return View();
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