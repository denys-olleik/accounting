using Accounting.Business;
using Accounting.Common;
using Accounting.CustomAttributes;
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
    private readonly InvitationService _invitationService;

    public UserController(
      RequestContext requestContext,
      UserOrganizationService userOrganizationService,
      UserService userService,
      SecretService secretService,
      InvitationService invitationService)
    {
      _userOrganizationService = new UserOrganizationService(requestContext.DatabaseName);
      _userService = new UserService(requestContext.DatabaseName);
      _secretService = new SecretService(requestContext.DatabaseName);
      _invitationService = new InvitationService(requestContext.DatabaseName);
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
    public async Task<ActionResult> Create(CreateUserViewModel model)
    {
      CreateUserViewModelValidator validator = new CreateUserViewModelValidator();
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      EmailService emailService = new EmailService(_secretService);
      using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        User user = await _userService.CreateAsync(new User()
        {
          Email = model.Email,
          FirstName = model.FirstName,
          LastName = model.LastName,
          CreatedById = GetUserId()
        });

        await _userOrganizationService.CreateAsync(new UserOrganization()
        {
          OrganizationId = GetOrganizationId(),
          UserId = user.UserID
        });

        if (model.SendInvitationEmail)
        {
          Invitation invitation = await _invitationService.CreatAsync(new Invitation()
          {
            Email = user.Email,
            Guid = Guid.NewGuid(),
            FirstName = model.FirstName,
            LastName = model.LastName,
            UserId = user.UserID,
            CreatedById = GetUserId(),
            OrganizationId = GetOrganizationId(),
            Expiration = DateTime.UtcNow + TimeSpan.FromMinutes(ConfigurationSingleton.Instance.InvitationExpirationMinutes)
          });

          await emailService.SendInvitationEmailAsync(invitation, GetBaseUrl());
        }

        scope.Complete();
      }

      return RedirectToAction("Users");
    }

    [HttpGet]
    [Route("details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
      User user = (await _userOrganizationService.GetAsync(id, GetOrganizationId(), null)).User!;

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
      _userService = new UserService(requestContext.DatabaseName);
    }

    [HttpGet("get-users")]
    public async Task<IActionResult> GetUsers(
      int page = 1,
      int pageSize = 10)
    {
      var (users, nextPage) = await _userService.GetAllAsync(
          page,
          pageSize);

      GetUsersViewModel getUsersViewModel = new GetUsersViewModel()
      {
        Users = users.Select(u => new GetUsersViewModel.UserViewModel()
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

      GetUsersViewModel getUsersViewModel = new GetUsersViewModel()
      {
        Users = users.Select(u => new GetUsersViewModel.UserViewModel()
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