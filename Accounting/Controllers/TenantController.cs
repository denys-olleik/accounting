using Microsoft.AspNetCore.Mvc;
using Accounting.Service;
using FluentValidation.Results;
using System.Transactions;
using Accounting.Business;
using FluentValidation;
using Accounting.Models.Tenant;
using Accounting.CustomAttributes;
using Accounting.Common;
using Accounting.Models.TenantViewModels;
using Accounting.Models.UserViewModels;

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
    private readonly UserService _userService;
    private readonly OrganizationService _organizationService;
    private readonly UserOrganizationService _userOrganizationService;


    public TenantController(
      TenantService tenantService,
      CloudServices cloudServices,
      SecretService secretService,
      DatabaseService databaseService,
      UserService userService,
      OrganizationService organizationService,
      UserOrganizationService userOrganizationService)
    {
      _tenantService = tenantService;
      _cloudServices = cloudServices;
      _secretService = secretService;
      _databaseService = databaseService;
      _userService = userService;
      _organizationService = organizationService;
      _userOrganizationService = userOrganizationService;
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

    [Route("create-user/{tenantId}")]
    [HttpGet]
    public async Task<IActionResult> CreateUser(string tenantId)
    {
      Tenant tenant = await _tenantService.GetAsync(int.Parse(tenantId));

      if (tenant == null)
      {
        return NotFound();
      }

      CreateUserViewModel model = new CreateUserViewModel();
      model.TenantId = tenant.TenantID;

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

      var validator = new CreateUserViewModel.CreateUserViewModelValidator(_userService);
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      User user = await _userService.CreateAsync(new User()
      {
        Email = model.Email,
        FirstName = model.FirstName,
        LastName = model.LastName,
        Password = !string.IsNullOrEmpty(model.Password)
          ? PasswordStorage.CreateHash(model.Password) : null
      }, tenant.DatabaseName!);

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

    [Route("add-user-orgnization/{tenantId}")]
    [HttpGet]
    public async Task<IActionResult> AddUserOrganization(string tenantId)
    {
      AddUserOrganizationViewModel model = new AddUserOrganizationViewModel();
      model.TenantId = int.Parse(tenantId);

      return View(model);
    }

    [Route("add-user-orgnization/{tenantId}")]
    [HttpPost]
    public async Task<IActionResult> AddUserOrganization(
      AddUserOrganizationViewModel model,
      string tenantId)
    {
      Tenant tenant = await _tenantService.GetAsync(int.Parse(tenantId));

      if (tenant == null)
      {
        model.ValidationResult.Errors.Add(new ValidationFailure("TenantId", "Tenant not found."));
        return View(model);
      }

      var validator = new AddUserOrganizationViewModel.AddUserOrganizationViewModelValidator(
        _userService,
        _organizationService,
        _tenantService,
        tenantId);
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      User user = null!;
      Organization organization = null!;

      if (model.InheritUser)
      {
        var (existingUser, tenantExistingUserBelongsTo) = await _userService.GetAsync(model.Email!);

        if (tenantExistingUserBelongsTo.TenantID != tenant.TenantID)
        {
          user = await _userService.CreateAsync(new User()
          {
            Email = model.Email,
            FirstName = existingUser.FirstName,
            LastName = existingUser.LastName,
            Password = !string.IsNullOrEmpty(existingUser.Password)
              ? PasswordStorage.CreateHash(existingUser.Password) : null
          }, tenant.DatabaseName!);
        }
        else
        {
          user = existingUser;
        }
      }
      else
      {
        user = await _userService.CreateAsync(new User()
        {
          Email = model.Email,
          FirstName = model.FirstName,
          LastName = model.LastName,
          Password = !string.IsNullOrEmpty(model.Password)
            ? PasswordStorage.CreateHash(model.Password) : null
        }, tenant.DatabaseName!);
      }

      if (model.InheritOrganization)
      {
        organization = await _organizationService.GetAsync(
          model.OrganizationName!,
          tenant.DatabaseName!);
      }
      else
      {
        organization = await _organizationService.CreateAsync(
          model.OrganizationName!,
          tenant.DatabaseName!);
      }

      UserOrganization userOrganization = await _userOrganizationService
        .GetAsync(
          user.UserID,
          organization.OrganizationID,
          tenant.TenantID);

      if (userOrganization == null)
      {
        await _userOrganizationService.CreateAsync(new UserOrganization()
        {
          UserId = user.UserID,
          OrganizationId = organization.OrganizationID
        }, tenant.DatabaseName!);
      }
      else
      {
        model.ValidationResult.Errors.Add(new ValidationFailure("Email", "User is already a member of the organization."));
        return View(model);
      }

      await _userService.UpdatePasswordAllTenantsAsync(user.Email!, user.Password!);

      return RedirectToAction("Tenants");
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
      model.OrganizationId = GetOrganizationId();

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

    public TenantApiController(
      TenantService tenantService,
      UserOrganizationService userOrganizationService)
    {
      _tenantService = tenantService;
      _userOrganizationService = userOrganizationService;
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
  }
}

namespace Accounting.Models.Tenant
{
  public class GetUserOrganizationsViewModel
  {
    public List<UserOrganization>? UserOrganizations { get; set; }

    public class UserOrganization
    {
      public int UserOrganizationID { get; set; }
      public int UserID { get; set; }
      public UserViewModel? User { get; set; }
      public int OrganizationID { get; set; }
      public OrganizationViewModel? Organization { get; set; }
    }

    public class OrganizationViewModel
    {
      public int OrganizationID { get; set; }
      public string? Name { get; set; }
    }

    public class UserViewModel
    {
      public int UserID { get; set; }
      public string? Email { get; set; }
    }
  }

  public class DeleteTenantViewModel
  {
    public int TenantId { get; set; }
    public bool DeleteDatabase { get; set; }
  }

  public class TenantUsersViewModel()
  {
    public int TenantId { get; set; }
  }

  public class AddUserOrganizationViewModel
  {
    public int TenantId { get; set; }

    private string? _email;
    public string? Email
    {
      get => _email;
      set => _email = value?.Trim().ToLower();
    }

    public bool InheritUser { get; set; }

    private string? _firstName;
    public string? FirstName
    {
      get => _firstName;
      set => _firstName = value?.Trim();
    }

    private string? _lastName;
    public string? LastName
    {
      get => _lastName;
      set => _lastName = value?.Trim();
    }

    private string? _password;
    public string? Password
    {
      get => _password;
      set => _password = value?.Trim();
    }

    private string? _organizationName;
    public string? OrganizationName
    {
      get => _organizationName;
      set => _organizationName = value?.Trim();
    }

    public bool InheritOrganization { get; set; }

    public ValidationResult? ValidationResult { get; set; } = new ValidationResult();

    public class AddUserOrganizationViewModelValidator : AbstractValidator<AddUserOrganizationViewModel>
    {
      private readonly UserService _userService;
      private readonly OrganizationService _organizationService;
      private readonly TenantService _tenantService;
      private readonly string _tenantId;

      public AddUserOrganizationViewModelValidator(
        UserService userService,
        OrganizationService organizationService,
        TenantService tenantService,
        string tenantId)
      {
        _userService = userService;
        _organizationService = organizationService;
        _tenantService = tenantService;
        _tenantId = tenantId;

        RuleFor(x => x.Email)
          .NotEmpty().WithMessage("Email is required.")
          .EmailAddress().WithMessage("Invalid email format.")
          .DependentRules(() =>
          {
            RuleFor(x => x)
              .MustAsync(async (model, cancellationToken) =>
              {
                var (existingUser, tenantExistingUserBelongsTo) = await _userService.GetAsync(model.Email!);
                return model.InheritUser ? existingUser != null : existingUser == null;
              })
              .WithMessage(model =>
                model.InheritUser ? "User does not exist to inherit." : "A user with this email already exists. Inherit user instead.");
          });

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .When(x => !x.InheritUser);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .When(x => !x.InheritUser);

        RuleFor(x => x.OrganizationName)
            .NotEmpty().WithMessage("Organization name is required.")
            .DependentRules(() =>
            {
              RuleFor(x => x)
                  .MustAsync(async (model, cancellationToken) =>
                  {
                    var tenant = await _tenantService.GetAsync(Convert.ToInt32(_tenantId));
                    var existingOrganization = await _organizationService.GetAsync(model.OrganizationName!, tenant.DatabaseName);
                    return model.InheritOrganization ? existingOrganization != null : existingOrganization == null;
                  })
                  .WithMessage(model =>
                      model.InheritOrganization ? "Organization does not exist to inherit." : "This organization already exists. Inherit organization instead.");
            });
      }
    }
  }

  public class ChooseTenantOrganizationViewModel
  {
    public string? OrganizationPublicId { get; set; }
    public string? TenantPublicId { get; set; }

    public ValidationResult? ValidationResult { get; set; }
  }

  public class ProvisionTenantViewModel
  {
    public string? Email { get; set; }
    public bool Shared { get; set; }
    public string? FullyQualifiedDomainName { get; set; }
    public int OrganizationId { get; set; }

    public ValidationResult? ValidationResult { get; set; }

    public class ProvisionTenantViewModelValidator
     : AbstractValidator<ProvisionTenantViewModel>
    {
      private readonly TenantService _tenantService;
      private readonly SecretService _secretService;

      public ProvisionTenantViewModelValidator(
          TenantService tenantService,
          SecretService secretService)
      {
        _tenantService = tenantService;
        _secretService = secretService;

        RuleFor(x => x.Email)
          .NotEmpty()
          .WithMessage("Email is required.")
          .DependentRules(() =>
          {
            RuleFor(x => x.Email)
              .EmailAddress()
              .WithMessage("Invalid email format.")
              .DependentRules(() =>
              {
                RuleFor(x => x)
                  .MustAsync(async (model, cancellation) =>
                  {
                    return !await _tenantService.ExistsAsync(model.Email!);
                  })
                  .WithMessage("A tenant with this email already exists.");
              });
          });

        RuleFor(x => x)
          .MustAsync(async (model, cancellation) =>
              await HasRequiredSecretsAsync(model.OrganizationId, model.Shared))
          .WithMessage("The required secret keys are not available for provisioning a tenant.");

        RuleFor(x => x.FullyQualifiedDomainName)
          .NotEmpty()
          .When(x => !x.Shared)
          .WithMessage("'Fully Qualified Domain Name' is required when 'Shared' is not selected.");
      }

      private async Task<bool> HasRequiredSecretsAsync(int organizationId, bool isShared)
      {
        var emailSecret = await _secretService.GetAsync(
            Secret.SecretTypeConstants.Email,
            organizationId);

        if (isShared)
        {
          return emailSecret != null;
        }
        else
        {
          var cloudSecret = await _secretService.GetAsync(
              Secret.SecretTypeConstants.Cloud,
              organizationId);
          return emailSecret != null && cloudSecret != null;
        }
      }
    }
  }
}