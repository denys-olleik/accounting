using Accounting.Business;
using Accounting.Common;
using Accounting.CustomAttributes;
using Accounting.Models.TenantViewModels;
using Accounting.Models.UserViewModels;
using Accounting.Service;
using Accounting.Validators;
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

    public UserController(
      RequestContext requestContext,
      UserOrganizationService userOrganizationService,
      UserService userService,
      SecretService secretService)
    {
      _userOrganizationService = new UserOrganizationService(requestContext.DatabaseName!, requestContext.DatabasePassword!);
      _userService = new UserService(requestContext.DatabaseName!, requestContext.DatabasePassword!);
      _secretService = new SecretService(requestContext.DatabaseName!, requestContext.DatabasePassword!);
    }

    [HttpGet]
    [Route("delete/{userId}")]
    public async Task<IActionResult> Delete(int userId)
    {
      var userOrg = await _userOrganizationService.GetAsync(userId, GetOrganizationId());

      if (userOrg?.User == null)
        return NotFound();

      return View(new Models.UserViewModels.DeleteUserViewModel
      {
        UserID = userOrg.User.UserID,
        Email = userOrg.User.Email,
        FirstName = userOrg.User.FirstName,
        LastName = userOrg.User.LastName
      });
    }

    [HttpPost]
    [Route("delete/{userId}")]
    public async Task<IActionResult> Delete(Models.UserViewModels.DeleteUserViewModel model)
    {
      var userOrg = await _userOrganizationService.GetAsync(model.UserID, GetOrganizationId());

      if (userOrg?.User == null)
        return NotFound();

      var validator = new Models.UserViewModels.DeleteUserViewModel.DeleteUserViewModelValidator();
      var validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        await _userOrganizationService.DeleteAsync(model.UserID, GetOrganizationId());
        scope.Complete();
      }

      return RedirectToAction("Users");
    }

    [HttpGet]
    [Route("users")]
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
    public async Task<ActionResult> Create(Models.TenantViewModels.CreateUserViewModel model)
    {
      Models.TenantViewModels.CreateUserViewModel.CreateUserViewModelValidator validator = new Models.TenantViewModels.CreateUserViewModel.CreateUserViewModelValidator();
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