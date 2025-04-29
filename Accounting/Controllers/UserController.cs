using Accounting.Business;
using Accounting.Common;
using Accounting.CustomAttributes;
using Accounting.Models.UserViewModels;
using Accounting.Service;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("u")]
  public class UserController : BaseController
  {
    private readonly UserOrganizationService _userOrganizationService;
    private readonly UserService _userService;
    private readonly SecretService _secretService;
    private readonly TenantService _tenantService;
    private readonly ClaimService _claimService;

    public UserController(
      RequestContext requestContext,
      UserOrganizationService userOrganizationService,
      UserService userService,
      SecretService secretService,
      TenantService tenantService,
      ClaimService claimService)
    {
      _userOrganizationService = new UserOrganizationService(requestContext.DatabaseName!, requestContext.DatabasePassword!);
      _userService = new UserService(requestContext.DatabaseName!, requestContext.DatabasePassword!);
      _secretService = new SecretService(requestContext.DatabaseName!, requestContext.DatabasePassword!);
      _tenantService = new TenantService();
      _claimService = new ClaimService(requestContext.DatabaseName!, requestContext.DatabasePassword!);
    }

    [HttpGet]
    [Route("update-email/{id}")]
    public async Task<IActionResult> UpdateEmail(int id)
    {
      var user = await _userService.GetAsync(id);
      if (user == null) return NotFound();

      if (user.UserID != GetUserId())
      {
        return Unauthorized();
      }

      var model = new UpdateEmailViewModel
      {
        UserID = user.UserID,
        CurrentEmail = user.Email,
        NewEmail = string.Empty
      };

      return View(model);
    }

    [HttpPost]
    [Route("update-email/{id}")]
    public async Task<IActionResult> UpdateEmail(UpdateEmailViewModel model, int id)
    {
      var user = await _userService.GetAsync(id);
      if (user == null) return NotFound();

      if (user.UserID != GetUserId())
      {
        return Unauthorized();
      }

      var validator = new UpdateEmailViewModel.UpdateEmailViewModelValidator();
      var validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      var existingUser = await _userService.GetAsync(model.NewEmail);
      if (existingUser != null)
      {
        model.ValidationResult = new ValidationResult(new List<ValidationFailure>()
        {
            new ValidationFailure("NewEmail", "Email already exists.")
        });
        model.UserID = id;
        return View(model);
      }

      await _tenantService.UpdateUserEmailAsync(user.Email, model.NewEmail);

      return RedirectToAction("Users", new { page = 1, pageSize = 2 });
    }

    [HttpGet]
    [Route("update/{userId}")]
    public async Task<IActionResult> UpdateUser(int userId)
    {
      UserService _userService = new UserService(GetDatabaseName(), GetDatabasePassword());
      User user = await _userService.GetAsync(userId);
      if (user == null) return NotFound();

      OrganizationService _organizationService = new OrganizationService(GetDatabaseName(), GetDatabasePassword());
      var organizations = await _organizationService.GetAllAsync(GetDatabaseName(), GetDatabasePassword());
      UserOrganizationService _userOrganizationService = new UserOrganizationService(GetDatabaseName(), GetDatabasePassword());
      var userOrganizations = await _userOrganizationService.GetByUserIdAsync(user.UserID, GetDatabaseName(), GetDatabasePassword());

      var selectedRoles = await _claimService.GetUserRolesAsync(user.UserID, GetOrganizationId(), Claim.CustomClaimTypeConstants.Role);

      var viewModel = new Models.UserViewModels.UpdateUserViewModel
      {
        UserID = user.UserID,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        AvailableOrganizations = organizations.Select(x => new Models.UserViewModels.UpdateUserViewModel.OrganizationViewModel
        {
          OrganizationID = x.OrganizationID,
          Name = x.Name
        }).ToList(),
        AvailableRoles = new List<string>
        {
          UserRoleClaimConstants.TenantManager,
        },
        SelectedOrganizationIdsCsv = string.Join(',', userOrganizations.Select(x => x.OrganizationID)),
        CurrentRequestingUserId = GetUserId(),
        SelectedRoles = selectedRoles
      };

      return View(viewModel);
    }

    [HttpPost]
    [Route("update/{userId}")]
    public async Task<IActionResult> UpdateUser(UpdateUserViewModel model)
    {
      var user = await _userService.GetAsync(model.UserID);
      if (user == null) return NotFound();

      var validator = new UpdateUserViewModel.UpdateUserViewModelValidator();
      var validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      string currentDatabaseName = GetDatabaseName();
      int currentOrganizationId = GetOrganizationId();

      var selectedOrganizationIds = (model.SelectedOrganizationIdsCsv?.Split(',') ?? Array.Empty<string>())
        .Where(id => !string.IsNullOrWhiteSpace(id))
        .Select(id => int.Parse(id.Trim()))
        .ToList();

      var currentUserId = GetUserId();

      if (user.UserID == currentUserId && !selectedOrganizationIds.Contains(currentOrganizationId))
      {
        return Unauthorized("Cannot un-associate yourself from the current organization.");
      }

      if (user.UserID == GetUserId())
      {
        user.Email = model.Email;
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        await _tenantService.UpdateUserAsync(user.Email, model.FirstName, model.LastName);
      }

      await _userOrganizationService.UpdateUserOrganizationsAsync(
          user.UserID,
          selectedOrganizationIds,
          GetDatabaseName(),
          GetDatabasePassword()
      );

      return RedirectToAction("Users");
    }

    [Route("users")]
    [HttpGet]
    public IActionResult Users(int page = 1, int pageSize = 2)
    {
      var refererHeader = Request.Headers["Referer"];

      var usersViewModel = new UsersPaginatedViewModel()
      {
        Page = page,
        PageSize = pageSize,
        RememberPageSize = string.IsNullOrEmpty(refererHeader)
      };

      return View(usersViewModel);
    }

    [HttpGet]
    [Route("create")]
    public async Task<IActionResult> Create()
    {
      return View();
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult> Create(Models.UserViewModels.CreateUserViewModel model)
    {
      Models.UserViewModels.CreateUserViewModel.CreateUserViewModelValidator validator = new();
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      User existingUser = await _userService.GetAsync(model.Email);
      if (existingUser != null)
      {
        model.ValidationResult = new ValidationResult(new List<ValidationFailure>()
        {
          new ValidationFailure("Email", "Email already exists.")
        });
        return View(model);
      }

      var (existingUser2, tenant) = await _userService.GetFirstOfAnyTenantAsync(model.Email);

      EmailService emailService = new EmailService(_secretService);
      using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        User user = await _userService.CreateAsync(new User()
        {
          Email = model.Email,
          FirstName = existingUser2?.FirstName,
          LastName = existingUser2?.LastName,
          CreatedById = GetUserId()
        });

        await _userOrganizationService.CreateAsync(new UserOrganization()
        {
          OrganizationId = GetOrganizationId(),
          UserId = user.UserID
        });

        scope.Complete();
      }

      return RedirectToAction("Users");
    }

    [HttpGet]
    [Route("details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
      User user = (await _userOrganizationService.GetAsync(id, GetOrganizationId())).User!;

      if (user == null)
      {
        return NotFound();
      }

      UserDetailsViewModel userDetailsViewModel = new UserDetailsViewModel();
      userDetailsViewModel.Email = user.Email;
      userDetailsViewModel.FirstName = user.FirstName;
      userDetailsViewModel.LastName = user.LastName;
      userDetailsViewModel.CreatedById = user.CreatedById;
      userDetailsViewModel.Created = user.Created;

      return View(userDetailsViewModel);
    }

    [HttpGet]
    [Route("update-password")]
    public async Task<IActionResult> UpdatePassword()
    {
      return View();
    }

    [HttpPost]
    [Route("update-password")]
    public async Task<IActionResult> UpdatePassword(UpdatePasswordViewModel model)
    {
      UpdatePasswordViewModel.UpdatePasswordViewModelValidator validator
        = new UpdatePasswordViewModel.UpdatePasswordViewModelValidator();
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      await _userService.UpdatePasswordAllTenantsAsync(GetEmail(), PasswordStorage.CreateHash(model.NewPassword));

      return RedirectToAction("Users");
    }
  }

  [AuthorizeWithOrganizationId]
  [ApiController]
  [Route("api/user")]
  public class UserApiController : BaseController
  {
    private readonly UserService _userService;

    public UserApiController(RequestContext requestContext, UserService userService)
    {
      _userService = new UserService(requestContext.DatabaseName, requestContext.DatabasePassword);
    }

    [HttpGet("get-users")]
    public async Task<IActionResult> GetUsers(
      int page = 1,
      int pageSize = 10)
    {
      var (users, nextPage) = await _userService.GetAllAsync(
          page,
          pageSize);

      Models.UserViewModels.GetUsersViewModel getUsersViewModel = new Models.UserViewModels.GetUsersViewModel()
      {
        Users = users.Select(u => new Models.UserViewModels.GetUsersViewModel.UserViewModel()
        {
          UserID = u.UserID,
          RowNumber = u.RowNumber,
          FirstName = u.FirstName,
          LastName = u.LastName,
          Email = u.Email
        }).ToList(),
        Page = page,
        NextPage = nextPage
      };

      return Ok(getUsersViewModel);
    }

    [HttpGet("get-users-filtered")]
    public async Task<IActionResult> GetUsersFiltered(
      string search = null,
      int page = 1,
      int pageSize = 10)
    {
      var users = await _userService.GetFilteredAsync(search);

      Models.UserViewModels.GetUsersViewModel getUsersViewModel = new Models.UserViewModels.GetUsersViewModel()
      {
        Users = users.Select(u => new Models.UserViewModels.GetUsersViewModel.UserViewModel()
        {
          UserID = u.UserID,
          RowNumber = u.RowNumber,
          FirstName = u.FirstName,
          LastName = u.LastName,
          Email = u.Email
        }).ToList()
      };

      return Ok(getUsersViewModel);
    }
  }
}