using Accounting.Business;
using Accounting.Common;
using Accounting.Helpers;
using Accounting.Models.UserAccountViewModels;
using Accounting.Service;
using Accounting.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Transactions;

namespace Accounting.Controllers
{
  [Authorize]
  [Route("user-account")]
  public class UserAccountController : BaseController
  {
    private readonly OrganizationService _organizationService;
    private readonly UserOrganizationService _userOrganizationService;
    private readonly UserService _userService;
    private readonly LoginWithoutPasswordService _loginWithoutPasswordService;
    private readonly EmailService _emailService;
    private readonly SecretService _secretService;
    private readonly TenantService _tenantService;
    private readonly ClaimService _claimService;

    public UserAccountController(
      RequestContext requestContext,
      OrganizationService organizationService,
      UserOrganizationService userOrganizationService,
      UserService userService,
      LoginWithoutPasswordService loginWithoutPasswordService,
      EmailService emailService,
      SecretService secretService,
      TenantService tenantService,
      ClaimService claimService)
    {
      _organizationService = new OrganizationService(requestContext.DatabaseName, requestContext.DatabasePassword);
      _userOrganizationService = new UserOrganizationService(requestContext.DatabaseName, requestContext.DatabasePassword);
      _userService = new UserService(requestContext.DatabaseName, requestContext.DatabasePassword);
      _loginWithoutPasswordService = new LoginWithoutPasswordService(requestContext.DatabaseName, requestContext.DatabasePassword);
      _secretService = new SecretService(requestContext.DatabaseName, requestContext.DatabasePassword);
      _emailService = new EmailService(_secretService);
      _tenantService = new TenantService(requestContext.DatabaseName, requestContext.DatabasePassword);
      _claimService = new ClaimService(requestContext.DatabaseName, requestContext.DatabasePassword);
    }

    [HttpGet]
    [Route("profile")]
    public async Task<IActionResult> Profile()
    {
      return View();
    }

    [AllowAnonymous]
    [Route("login")]
    [HttpGet]
    public async Task<IActionResult> Login()
    {
      return View();
    }

    private async Task<List<string>> GetRolesAsync(UserOrganization userOrganization = null!, List<string> addTheseRoles = null!)
    {
      List<string> roles = new();

      if (userOrganization != null)
      {
        // Logic to read user roles from the database using userOrganization
        // Example: roles = await _roleService.GetRolesByUserOrganization(userOrganization);
      }

      if (ConfigurationSingleton.Instance.TenantManagement == true)
      {
        roles.Add(ConfigurationSingleton.ConfigurationConstants.TenantManagement);
      }

      // add addTheseRoles to roles
      if (addTheseRoles != null && addTheseRoles.Count > 0)
      {
        roles.AddRange(addTheseRoles);
      }

      return roles;
    }

    [AllowAnonymous]
    [Route("login")]
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
      LoginViewModelValidator loginViewModelValidator = new LoginViewModelValidator();
      ValidationResult validationResult = await loginViewModelValidator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      var (existingUser, tenantExistingUserBelongsTo) = await _userService.GetFirstOfAnyTenantAsync(model.Email!);

      if (
        existingUser != null
        && (!string.IsNullOrEmpty(existingUser.Password) && !string.IsNullOrEmpty(model.Password))
        && PasswordStorage.VerifyPassword(model.Password, existingUser.Password))
      {
        Business.Claim tenantManagerClaim = await _claimService.GetAsync(
        existingUser.UserID,
        tenantExistingUserBelongsTo.DatabaseName,
        UserRoleClaimConstants.TenantManager);

        var rolesToAdd = tenantManagerClaim != null
          ? new List<string>() { tenantManagerClaim.ClaimValue }
          : new List<string>();

        var roles = await ComposeRoles(
          existingUser.UserID,
          tenantExistingUserBelongsTo.DatabaseName,
          rolesToAdd);

        ClaimsPrincipal claimsPrincipal = AuthenticationHelper.CreateClaimsPrincipal(
          existingUser,
          tenantExistingUserBelongsTo.TenantID,
          roles,
          null,
          null,
          tenantExistingUserBelongsTo.DatabaseName,
          tenantExistingUserBelongsTo.DatabasePassword);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
          claimsPrincipal,
          new AuthenticationProperties()
          {
            IsPersistent = true
          });

        return RedirectToAction("ChooseOrganization", "UserAccount");
      }
      else if (existingUser != null && string.IsNullOrEmpty(model.Password))
      {
        Secret? emailApiKeySecret = await _secretService.GetAsync(Secret.SecretTypeConstants.Email, 1);
        Secret? noReplySecret = await _secretService.GetAsync(Secret.SecretTypeConstants.NoReply, 1);

        if (emailApiKeySecret == null || noReplySecret == null)
        {
          model.ValidationResult = new ValidationResult(new List<ValidationFailure>()
      {
          new ValidationFailure("Email", "Password-less login requires an API key and 'no-reply' secrets to be set up.")
      });
          return View(model);
        }

        LoginWithoutPassword loginWithoutPassword;

        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
          await _loginWithoutPasswordService.DeleteAsync(model.Email);
          loginWithoutPassword = await _loginWithoutPasswordService.CreateAsync(model.Email!);
          await _emailService.SendLoginWithoutPasswordAsync(loginWithoutPassword);
          scope.Complete();
          return RedirectToAction("LoginWithoutPassword", "UserAccount", new { Email = model.Email });
        }
      }
      else
      {
        model.ValidationResult = new ValidationResult(new List<ValidationFailure>()
      {
        new ValidationFailure("Email", "'Email' or 'password' is incorrect.")
      });
        return View(model);
      }
    }

    [AllowAnonymous]
    [Route("login-without-password/{email}")]
    [HttpGet]
    public IActionResult LoginWithoutPassword(string email)
    {
      ViewBag.Email = email;
      return View();
    }

    [AllowAnonymous]
    [Route("login-without-password/{email}")]
    [HttpPost]
    public async Task<IActionResult> LoginWithoutPassword(LoginWithoutPasswordViewModel model, string email)
    {
      LoginWithoutPasswordViewModelValidator validator
        = new LoginWithoutPasswordViewModelValidator(_loginWithoutPasswordService);
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      var (existingUser, tenantExistingUserBelongsTo) = await _userService.GetFirstOfAnyTenantAsync(model.Email!);

      ClaimsPrincipal claimsPrincipal = AuthenticationHelper.CreateClaimsPrincipal(
        existingUser,
        tenantExistingUserBelongsTo.TenantID,
        await GetRolesAsync(),
        null,
        null,
        tenantExistingUserBelongsTo.DatabaseName,
        tenantExistingUserBelongsTo.DatabasePassword);

      await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        claimsPrincipal,
        new AuthenticationProperties() { IsPersistent = true }
      );

      LoginWithoutPassword loginWithoutPassword = await _loginWithoutPasswordService.GetAsync(model.Email)!;
      await _loginWithoutPasswordService.DeleteAsync(loginWithoutPassword);

      return RedirectToAction("ChooseOrganization", "UserAccount");
    }

    [HttpGet]
    [Route("choose-organization")]
    public async Task<IActionResult> ChooseOrganization()
    {
      List<(Organization Organization, Tenant? Tenant)> organizationTuples = await _userOrganizationService.GetByEmailAsync(GetEmail(), true);

      ChooseOrganizationViewModel model = new ChooseOrganizationViewModel()
      {
        Organizations = organizationTuples.Select(x => new OrganizationViewModel()
        {
          OrganizationId = x.Organization.OrganizationID,
          Name = x.Organization.Name!,
          TenantId = x.Tenant?.TenantID
        }).ToList()
      };

      return View(model);
    }

    [HttpPost]
    [Route("choose-organization")]
    public async Task<IActionResult> ChooseOrganization(ChooseOrganizationViewModel model)
    {
      List<(Organization Organization, Tenant? Tenant)> organizationTuples = await _userOrganizationService.GetByEmailAsync(GetEmail(), true);

      model.Organizations = organizationTuples.Select(x => new OrganizationViewModel()
      {
        OrganizationId = x.Organization.OrganizationID,
        Name = x.Organization.Name!,
        TenantId = x.Tenant?.TenantID
      }).ToList();

      if (model.SelectedOrganizationId == null || model.SelectedOrganizationId == 0)
      {
        model.ValidationResult.Errors.Add(new ValidationFailure("OrganizationId", "You must select an organization."));
        return View(model);
      }

      Tenant tenant = await _tenantService.GetAsync(model.SelectedTenantId!.Value);
      UserOrganization userOrganization
        = await _userOrganizationService
          .GetByEmailAsync(
            GetEmail(),
            model.SelectedOrganizationId,
            model.SelectedTenantId!.Value);

      if (userOrganization != null)
      {
        User user = userOrganization.User!;

        ClaimService claimService = new ClaimService(tenant.DatabaseName!, tenant.DatabasePassword!);
        List<string> userRoles = await claimService.GetUserRolesAsync(
          user.UserID,
          userOrganization.Organization.OrganizationID,
          Business.Claim.CustomClaimTypeConstants.Role);

        var roles = await ComposeRoles(
          user.UserID,
          tenant.DatabaseName, userRoles);

        ClaimsPrincipal claimsPrincipal = AuthenticationHelper.CreateClaimsPrincipal(
          user,
          tenant.TenantID,
          roles,
          userOrganization.Organization.OrganizationID,
          userOrganization.Organization.Name,
          tenant.DatabaseName,
          tenant.DatabasePassword);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
          claimsPrincipal,
          new AuthenticationProperties()
          {
            IsPersistent = true
          });

        return RedirectToAction("Index", "Home");
      }
      else
      {
        model.ValidationResult.Errors.Add(new ValidationFailure("OrganizationId", "You are not a member of this organization."));
        return View(model);
      }
    }

    [HttpPost]
    [Route("logout")]
    public async Task<IActionResult> Logout()
    {
      await HttpContext.SignOutAsync();

      return RedirectToAction("Index", "Home");
    }

    private async Task<List<string>> ComposeRoles(int userId, string databaseName, List<string>? additionalRoles = null)
    {
      var roles = new List<string>();

      if (ConfigurationSingleton.Instance.TenantManagement)
      {
        roles.Add(ConfigurationSingleton.ConfigurationConstants.TenantManagement);
      }

      if (additionalRoles != null && additionalRoles.Count > 0)
      {
        roles.AddRange(additionalRoles);
      }

      return roles;
    }
  }
}