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

    public UserAccountController(
      RequestContext requestContext,
      OrganizationService organizationService,
      UserOrganizationService userOrganizationService,
      UserService userService,
      LoginWithoutPasswordService loginWithoutPasswordService,
      EmailService emailService,
      SecretService secretService,
      TenantService tenantService)
    {
      _organizationService = new OrganizationService(requestContext.DatabaseName);
      _userOrganizationService = new UserOrganizationService(requestContext.DatabaseName);
      _userService = new UserService(requestContext.DatabaseName);
      _loginWithoutPasswordService = new LoginWithoutPasswordService(requestContext.DatabaseName);
      _secretService = new SecretService(requestContext.DatabaseName);
      _emailService = new EmailService(_secretService);
      _tenantService = new TenantService(requestContext.DatabaseName);
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
        ClaimsPrincipal claimsPrincipal = AuthenticationHelper.CreateClaimsPrincipal(existingUser);

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
        Secret? emailApiKeySecret = await _secretService.GetAsync(Secret.SecretTypeConstants.Email);
        Secret? noReplySecret = await _secretService.GetAsync(Secret.SecretTypeConstants.NoReply);

        if (emailApiKeySecret == null || noReplySecret == null)
        {
          model.ValidationResult = new ValidationResult(new List<ValidationFailure>()
          {
              new ValidationFailure("Email", "Passwordless login requires an API key and 'no-reply' secrets to be set up.")
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
      ClaimsPrincipal claimsPrincipal = AuthenticationHelper.CreateClaimsPrincipal(existingUser);

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

      User user = userOrganization.User!;

      if (userOrganization != null)
      {
        ClaimsPrincipal claimsPrincipal
          = AuthenticationHelper.CreateClaimsPrincipal(
            user,
            userOrganization.Organization.OrganizationID,
            userOrganization.Organization.Name,
            tenant.DatabaseName);

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
  }
}