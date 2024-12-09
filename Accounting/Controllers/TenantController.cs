using Microsoft.AspNetCore.Mvc;
using Accounting.Service;
using FluentValidation.Results;
using System.Transactions;
using Accounting.Business;
using FluentValidation;
using Accounting.Models.TenantViewModels;
using Accounting.CustomAttributes;
using Accounting.Common;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("tenant")]
  public class TenantController : BaseController
  {
    private readonly TenantService _tenantService;
    private readonly CloudServices _cloudServices;
    private readonly SecretService _secretService;
    private readonly DatabaseService _databaseService;
    private readonly OrganizationService _organizationService;
    private readonly UserOrganizationService _userOrganizationService;


    public TenantController(
      TenantService tenantService,
      CloudServices cloudServices,
      SecretService secretService,
      DatabaseService databaseService,
      OrganizationService organizationService,
      UserOrganizationService userOrganizationService,
      RequestContext requestContext)
    {
      _tenantService = new TenantService(requestContext.DatabaseName);
      _cloudServices = cloudServices;
      _secretService = new SecretService(requestContext.DatabaseName);
      _databaseService = databaseService;
      _organizationService = new OrganizationService(requestContext.DatabaseName);
      _userOrganizationService = new UserOrganizationService(requestContext.DatabaseName);
    }

    [Route("delete-user/{tenantId}/{userId}")]
    [HttpGet]
    public IActionResult DeleteUser(string tenantId, string userId)
    {
      throw new NotImplementedException("Get method for deleting a user is not implemented yet.");
    }

    [Route("delete-user/{tenantId}/{userId}")]
    [HttpPost]
    public IActionResult DeleteUser(DeleteUserViewModel model, string tenantId, string userId)
    {
      throw new NotImplementedException("Post method for deleting a user is not implemented yet.");
    }

    [Route("delete-organization/{tenantId}/{organizationId}")]
    [HttpGet]
    public async Task<IActionResult> DeleteOrganization(string tenantId, string organizationId)
    {
      Tenant tenant = await _tenantService.GetAsync(int.Parse(tenantId));
      if (tenant == null)
      {
        return NotFound();
      }

      Organization organization = await _organizationService.GetAsync(int.Parse(organizationId), tenant.DatabaseName!);
      if (organization == null)
      {
        return NotFound();
      }

      var model = new DeleteOrganizationViewModel
      {
        TenantId = tenant.TenantID,
        OrganizationId = organization.OrganizationID,
        OrganizationName = organization.Name
      };

      return View(model);
    }

    [Route("delete-organization/{tenantId}/{organizationId}")]
    [HttpPost]
    public async Task<IActionResult> DeleteOrganization(DeleteOrganizationViewModel model)
    {
      Tenant tenant = await _tenantService.GetAsync(model.TenantId);
      if (tenant == null)
      {
        return NotFound();
      }

      using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        await _userOrganizationService.DeleteByOrganizationIdAsync(model.OrganizationId, tenant.DatabaseName!);
        await _organizationService.DeleteAsync(model.OrganizationId, tenant.DatabaseName!);
        scope.Complete();
      }

      return RedirectToAction("Organizations", new { tenantId = model.TenantId });
    }

    [Route("update-organization/{tenantId}/{organizationId}")]
    [HttpGet]
    public async Task<IActionResult> UpdateOrganization(string tenantId, string organizationId)
    {
      Tenant tenant = await _tenantService.GetAsync(int.Parse(tenantId));
      if (tenant == null)
      {
        return NotFound();
      }

      Organization organization = await _organizationService.GetAsync(int.Parse(organizationId), tenant.DatabaseName!);
      if (organization == null)
      {
        return NotFound();
      }

      UpdateOrganizationViewModel model = new UpdateOrganizationViewModel
      {
        TenantId = tenant.TenantID,
        OrganizationID = organization.OrganizationID,
        Name = organization.Name
      };

      return View(model);
    }

    [Route("update-organization/{tenantId}/{organizationId}")]
    [HttpPost]
    public async Task<IActionResult> UpdateOrganization(string tenantId, string organizationId, UpdateOrganizationViewModel model)
    {
      Tenant tenant = await _tenantService.GetAsync(int.Parse(tenantId));
      if (tenant == null)
      {
        return NotFound();
      }
      Organization organization = await _organizationService.GetAsync(int.Parse(organizationId), tenant.DatabaseName!);
      if (organization == null)
      {
        return NotFound();
      }
      var validator = new UpdateOrganizationViewModel.UpdateOrganizationViewModelValidator();
      ValidationResult validationResult = await validator.ValidateAsync(model);
      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }
      organization.Name = model.Name;
      await _organizationService.UpdateAsync(organization.OrganizationID, model.Name, tenant.DatabaseName!);
      return RedirectToAction("Organizations", new { tenantId = tenant.TenantID });
    }

    [Route("organizations/{tenantId}")]
    [HttpGet]
    public async Task<IActionResult> Organizations(string tenantId)
    {
      Tenant tenant = await _tenantService.GetAsync(int.Parse(tenantId));

      if (tenant == null)
      {
        return NotFound();
      }

      OrganizationsViewModel model = new OrganizationsViewModel();
      model.TenantId = tenant.TenantID;

      List<Organization> organizations = await _organizationService.GetAllAsync(tenant.DatabaseName!);
      model.Organizations = organizations.Select(organizations => new OrganizationsViewModel.OrganizationViewModel
      {
        OrganizationID = organizations.OrganizationID,
        Name = organizations.Name
      }).ToList();

      model.Organizations = model.Organizations.OrderByDescending(x => x.OrganizationID).ToList();

      return View(model);
    }

    [Route("update-user/{tenantId}/{userId}")]
    [HttpGet]
    public async Task<IActionResult> UpdateUser(string tenantId, string userId)
    {
      Tenant tenant = await _tenantService.GetAsync(int.Parse(tenantId));

      if (tenant == null)
      {
        return NotFound();
      }

      UserService _userService = new UserService(tenant.DatabaseName);
      User user = await _userService.GetAsync(int.Parse(userId));

      if (user == null)
      {
        return NotFound();
      }

      var organizations = await _organizationService.GetAllAsync(tenant.DatabaseName!);
      var userOrganizations = await _userOrganizationService.GetByUserIdAsync(user.UserID, tenant.DatabaseName!);

      UpdateUserViewModel model = new UpdateUserViewModel
      {
        TenantId = tenant.TenantID,
        UserID = user.UserID,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        AvailableOrganizations = organizations.Select(x => new UpdateUserViewModel.OrganizationViewModel
        {
          OrganizationID = x.OrganizationID,
          Name = x.Name
        }).ToList(),
        SelectedOrganizationIdsCsv = string.Join(',', userOrganizations.Select(x => x.OrganizationID))
      };

      return View(model);
    }

    [Route("update-user/{tenantId}/{userId}")]
    [HttpPost]
    public async Task<IActionResult> UpdateUser(
      string tenantId,
      string userId,
      UpdateUserViewModel model)
    {
      Tenant tenant = await _tenantService.GetAsync(int.Parse(tenantId));
      if (tenant == null)
      {
        return NotFound();
      }

      UserService _userService = new UserService(tenant.DatabaseName);

      User user = await _userService.GetAsync(int.Parse(userId));
      if (user == null)
      {
        return NotFound();
      }

      var organizations = await _organizationService.GetAllAsync(tenant.DatabaseName!);
      model.AvailableOrganizations = organizations.Select(x => new UpdateUserViewModel.OrganizationViewModel
      {
        OrganizationID = x.OrganizationID,
        Name = x.Name
      }).ToList();

      var validator = new UpdateUserViewModel.UpdateUserViewModelValidator();
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      user.FirstName = model.FirstName;
      user.LastName = model.LastName;

      await _userService.UpdateAsync(user.Email, user.FirstName, user.LastName);

      // Handle the case when no organizations are selected
      var selectedOrganizationIds = !string.IsNullOrEmpty(model.SelectedOrganizationIdsCsv)
          ? model.SelectedOrganizationIdsCsv
              .Split(',', StringSplitOptions.RemoveEmptyEntries)
              .Select(int.Parse)
              .ToList()
          : new List<int>();

      await _userOrganizationService.UpdateUserOrganizationsAsync(user.UserID, selectedOrganizationIds, tenant.DatabaseName!);

      return RedirectToAction("TenantUsers", new { tenantId = tenant.TenantID });
    }

    [Route("create-user/{tenantId}")]
    [HttpGet]
    public async Task<IActionResult> CreateUser(string tenantId)
    {
      Tenant tenant = await _tenantService.GetAsync(int.Parse(tenantId));

      if (tenant == null)
      {
        return NotFound();
      }

      var organizations = await _organizationService.GetAllAsync(tenant.DatabaseName!);

      Models.TenantViewModels.CreateUserViewModel model = new Models.TenantViewModels.CreateUserViewModel
      {
        TenantId = tenant.TenantID,
        AvailableOrganizations = organizations.Select(x => new Models.TenantViewModels.CreateUserViewModel.OrganizationViewModel
        {
          OrganizationID = x.OrganizationID,
          Name = x.Name
        }).ToList()
      };

      return View(model);
    }

    [Route("create-user/{tenantId}")]
    [HttpPost]
    public async Task<IActionResult> CreateUser(
      CreateUserViewModel model,
      string tenantId)
    {
      Tenant tenant = await _tenantService.GetAsync(int.Parse(tenantId));

      if (tenant == null)
      {
        return NotFound();
      }

      var organizations = await _organizationService.GetAllAsync(tenant.DatabaseName!);
      model.AvailableOrganizations = organizations.Select(x => new CreateUserViewModel.OrganizationViewModel
      {
        OrganizationID = x.OrganizationID,
        Name = x.Name
      }).ToList();

      if (!string.IsNullOrEmpty(model.SelectedOrganizationIdsCsv))
      {
        model.SelectedOrganizationIdsCsv = string.Join(',',
          model.SelectedOrganizationIdsCsv.Split(',').Where(id => !string.IsNullOrEmpty(id)));
      }

      UserService _userService = new UserService(tenant.DatabaseName);
      User user = await _userService.GetAsync(model.Email);

      if (user != null)
      {
        model.ValidationResult.Errors.Add(new ValidationFailure("Email", "User already exists."));
        return View(model);
      }

      (User existingUser, Tenant tenantExistingUserBelongsTo) = await _userService.GetFirstOfAnyTenantAsync(model.Email!);

      if (existingUser != null)
      {
        model.ExistingUser = new CreateUserViewModel.ExistingUserViewModel()
        {
          UserID = existingUser.UserID,
          Email = existingUser.Email,
          FirstName = existingUser.FirstName,
          LastName = existingUser.LastName,
          Password = existingUser.Password
        };
      }

      var validator = new CreateUserViewModel.CreateUserViewModelValidator();
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      user = await _userService.CreateAsync(new User()
      {
        Email = model.Email,
        FirstName = model.ExistingUser?.FirstName ?? model.FirstName,
        LastName = model.ExistingUser?.LastName ?? model.LastName,
        Password = !string.IsNullOrEmpty(model.Password)
          ? PasswordStorage.CreateHash(model.Password)
          : null
      }, tenant.DatabaseName!);

      if (!string.IsNullOrEmpty(model.SelectedOrganizationIdsCsv))
      {
        var selectedOrganizationIds = model.SelectedOrganizationIdsCsv.Split(',').Select(int.Parse);
        foreach (var organizationId in selectedOrganizationIds)
        {
          await _userOrganizationService.CreateAsync(user.UserID, organizationId, tenant.DatabaseName!);
        }
      }

      await _userService.UpdatePasswordAllTenantsAsync(user.Email!, user.Password!);

      return RedirectToAction("Tenants");
    }

    [Route("create-organization/{tenantId}")]
    [HttpGet]
    public async Task<IActionResult> CreateOrganization(string tenantId)
    {
      Tenant tenant = await _tenantService.GetAsync(int.Parse(tenantId));

      if (tenant == null)
      {
        return NotFound();
      }

      CreateOrganizationViewModel model = new CreateOrganizationViewModel();
      model.TenantId = tenant.TenantID;

      return View(model);
    }

    [Route("create-organization/{tenantId}")]
    [HttpPost]
    public async Task<IActionResult> CreateOrganization(
      CreateOrganizationViewModel model,
      string tenantId)
    {
      Tenant tenant = await _tenantService.GetAsync(int.Parse(tenantId));

      if (tenant == null)
      {
        return NotFound();
      }

      var validator = new CreateOrganizationViewModel.CreateOrganizationViewModelValidator(
        _organizationService,
        tenant.DatabaseName!);
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      Organization organization = await _organizationService.CreateAsync(
        model.Name,
        tenant.DatabaseName!);

      return RedirectToAction("Organizations", "Tenant", new { tenantId });
    }

    [Route("delete/{tenantId}")]
    [HttpGet]
    public async Task<IActionResult> Delete(string tenantId, bool deleteDatabase = false)
    {
      Tenant tenant = await _tenantService.GetAsync(int.Parse(tenantId));

      if (tenant == null)
      {
        return NotFound();
      }

      DeleteTenantViewModel model = new DeleteTenantViewModel()
      {
        TenantId = tenant.TenantID,
        DeleteDatabase = deleteDatabase
      };

      return View(model);
    }

    [Route("delete/{tenantId}")]
    [HttpPost]
    public async Task<IActionResult> Delete(DeleteTenantViewModel model)
    {
      Tenant tenant = await _tenantService.GetAsync(model.TenantId);

      if (tenant == null)
      {
        return NotFound();
      }

      if (model.DeleteDatabase && !string.IsNullOrEmpty(tenant.DatabaseName))
      {
        await _databaseService.DeleteAsync(tenant.DatabaseName);
      }

      await _tenantService.DeleteAsync(tenant.TenantID);

      return RedirectToAction("Tenants");
    }

    [Route("provision-tenant")]
    [HttpGet]
    public IActionResult ProvisionTenant()
    {
      ProvisionTenantViewModel model = new ProvisionTenantViewModel();
      model.Shared = true;

      return View(model);
    }

    [Route("provision-tenant")]
    [HttpPost]
    public async Task<IActionResult> ProvisionTenant(
      ProvisionTenantViewModel model)
    {

      ProvisionTenantViewModel.ProvisionTenantViewModelValidator validator
        = new ProvisionTenantViewModel.ProvisionTenantViewModelValidator(_tenantService, _secretService);
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      if (model.Shared)
      {
        Tenant tenant;

        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
          tenant = await _tenantService.CreateAsync(new Tenant()
          {
            FullyQualifiedDomainName = model.FullyQualifiedDomainName,
            Email = model.Email,
          });

          scope.Complete();
        }

        string createSchemaScriptPath = Path.Combine(AppContext.BaseDirectory, "create-db-script-psql.sql");
        string createSchemaScript = System.IO.File.ReadAllText(createSchemaScriptPath);

        DatabaseThing database = await _databaseService.CreateDatabaseAsync(tenant.PublicId);
        await _databaseService.RunSQLScript(createSchemaScript, database.Name);
        await _tenantService.UpdateDatabaseName(tenant.TenantID, database.Name);
      }
      else
      {
        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
          Tenant tenant;

          tenant = await _tenantService.CreateAsync(new Tenant()
          {
            Email = model.Email,
            FullyQualifiedDomainName = model.FullyQualifiedDomainName
          });

          await _cloudServices.GetDigitalOceanService(
            _secretService,
            _tenantService,
            GetOrganizationId()).CreateDropletAsync(tenant);

          scope.Complete();
        }
      }

      return RedirectToAction("Tenants");
    }

    [Route("tenants")]
    [HttpGet]
    public IActionResult Tenants(
      int page = 1,
      int pageSize = 2)
    {
      var vm = new TenantsPaginatedViewModel()
      {
        Page = page,
        PageSize = pageSize,
      };

      return View(vm);
    }

    [Route("users/{tenantId}")]
    [HttpGet]
    public IActionResult TenantUsers(string tenantId)
    {
      TenantUsersViewModel model = new TenantUsersViewModel();
      model.TenantId = int.Parse(tenantId);

      return View(model);
    }
  }

  [AuthorizeWithOrganizationId]
  [Route("api/tenant")]
  [ApiController]
  public class TenantApiController : BaseController
  {
    private readonly TenantService _tenantService;
    private readonly UserOrganizationService _userOrganizationService;
    private readonly UserService _userService;

    public TenantApiController(
      TenantService tenantService,
      UserOrganizationService userOrganizationService,
      UserService userService)
    {
      _tenantService = tenantService;
      _userOrganizationService = userOrganizationService;
      _userService = userService;
    }

    [HttpGet("get-all-tenants")]
    public async Task<IActionResult> GetAllTenants(
      int page = 1,
      int pageSize = 2)
    {
      (List<Tenant> tenants, int? nextPage) =
        await _tenantService.GetAllAsync(
          page,
          pageSize);

      TenantViewModel ConvertToViewModel(Tenant tenant)
      {
        return new TenantViewModel
        {
          TenantID = tenant.TenantID,
          RowNumber = tenant.RowNumber,
          DatabaseName = tenant.DatabaseName,
          FullyQualifiedDomainName = tenant.FullyQualifiedDomainName,
          Email = tenant.Email,
          DropletId = tenant.DropletId,
          Ipv4 = tenant.Ipv4,
          SshPublic = !string.IsNullOrEmpty(tenant.SshPublic),
          SshPrivate = !string.IsNullOrEmpty(tenant.SshPrivate),
          Created = tenant.Created
        };
      }

      var viewModel = new GetAllTenantsViewModel
      {
        Tenants = tenants.Select(ConvertToViewModel).ToList(),
        Page = page,
        NextPage = nextPage,
        PageSize = pageSize
      };

      return Ok(viewModel);
    }

    [HttpGet("{tenantId}/user-organizations")]
    public async Task<IActionResult> GetUserOrganizations(int tenantId)
    {
      List<UserOrganization> userOrganizations = await _userOrganizationService.GetAllAsync(tenantId);

      GetUserOrganizationsViewModel model = new GetUserOrganizationsViewModel()
      {
        UserOrganizations = userOrganizations.Select(x => new GetUserOrganizationsViewModel.UserOrganization
        {
          UserOrganizationID = x.UserOrganizationID,
          UserID = x.UserId,
          User = new GetUserOrganizationsViewModel.UserViewModel
          {
            UserID = x.User!.UserID,
            Email = x.User!.Email
          },
          OrganizationID = x.OrganizationId,
          Organization = new GetUserOrganizationsViewModel.OrganizationViewModel
          {
            OrganizationID = x.Organization!.OrganizationID,
            Name = x.Organization!.Name
          }
        }).ToList()
      };

      return Ok(userOrganizations);
    }

    [HttpGet("{tenantId}/users")]
    public async Task<IActionResult> GetUsers(int tenantId)
    {
      Tenant tenant = await _tenantService.GetAsync(tenantId);

      if (tenant == null)
      {
        return NotFound();
      }

      List<User> users = await _userOrganizationService.GetUsersWithOrganizationsAsync(tenant.DatabaseName!);

      var model = new GetUsersViewModel
      {
        Users = users.Select(x => new GetUsersViewModel.UserViewModel
        {
          UserID = x.UserID,
          Email = x.Email!,
          Organizations = x.Organizations.Select(o => new GetUsersViewModel.OrganizationViewModel
          {
            OrganizationID = o.OrganizationID,
            Name = o.Name!
          }).ToList()
        }).ToList()
      };

      return Ok(model);
    }
  }
}