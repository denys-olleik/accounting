using Accounting.Business;
using Accounting.Common;
using Accounting.Models.UserAccountViewModels;
using Accounting.Service;
using Accounting.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using static Accounting.Business.Claim;

namespace Accounting.Controllers
{
  [Authorize]
  [Route("user-account")]
  public class UserAccountController : BaseController
  {
    private readonly OrganizationService _organizationService;
    private readonly UserOrganizationService _userOrganizationService;
    private readonly UserService _userService;

    public UserAccountController(
      OrganizationService organizationService, 
      UserOrganizationService userOrganizationService, 
      UserService userService)
    {
      _organizationService = organizationService;
      _userOrganizationService = userOrganizationService;
      _userService = userService;
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

      UserService userService = new UserService();
      User user = await userService.GetAsync(model.Email, true);

      if (
        user != null
        && !string.IsNullOrEmpty(user.Password)
        && PasswordStorage.VerifyPassword(model.Password, user.Password))
      {
        ClaimsPrincipal claimsPrincipal = CreateClaimsPricipal(user);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
          claimsPrincipal,
          new AuthenticationProperties()
          {
            IsPersistent = true
          });

        return RedirectToAction("ChooseOrganization", "UserAccount");
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

    [HttpGet]
    [Route("choose-organization")]
    public async Task<IActionResult> ChooseOrganization()
    {
      List<Organization> organizations = await _userOrganizationService.GetByUserIdAsync(GetUserId());

      ChooseOrganizationViewModel model = new ChooseOrganizationViewModel()
      {
        Organizations = organizations.Select(x => new SelectListItem()
        {
          Text = x.Name,
          Value = x.OrganizationID.ToString()
        }).ToList()
      };

      return View(model);
    }

    [HttpPost]
    [Route("choose-organization")]
    public async Task<IActionResult> ChooseOrganization(ChooseOrganizationViewModel model)
    {
      List<Organization> organizations = await _userOrganizationService.GetByUserIdAsync(GetUserId());

      model.Organizations = organizations.Select(x => new SelectListItem()
      {
        Text = x.Name,
        Value = x.OrganizationID.ToString()
      }).ToList();
      model.ValidationResult = new ValidationResult();

      if (model.OrganizationId == 0)
      {
        model.ValidationResult.Errors.Add(new ValidationFailure("OrganizationId", "You must select an organization."));
        return View(model);
      }

      UserOrganization userOrganization = await _userOrganizationService.GetAsync(GetUserId(), model.OrganizationId);

      User user = (await _userOrganizationService.GetAsync(GetUserId(), userOrganization.OrganizationId)).User!;

      if (userOrganization != null)
      {
        ClaimsPrincipal claimsPrincipal = CreateClaimsPricipal(user, userOrganization.OrganizationId, organizations.SingleOrDefault(x => x.OrganizationID == model.OrganizationId)!.Name);

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

    private ClaimsPrincipal CreateClaimsPricipal(User user, int? organizatonId = null, string? organizationName = null)
    {
      List<System.Security.Claims.Claim> claims = new List<System.Security.Claims.Claim>()
            {
                new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new System.Security.Claims.Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim()),
                new System.Security.Claims.Claim(ClaimTypes.Email, user.Email),
                new System.Security.Claims.Claim(CustomClaimTypeConstants.Password, user.Password)
            };

      if (organizatonId.HasValue)
      {
        claims.Add(new System.Security.Claims.Claim(CustomClaimTypeConstants.OrganizationId, organizatonId.Value.ToString()));
      }

      if (!string.IsNullOrEmpty(organizationName))
      {
        claims.Add(new System.Security.Claims.Claim(CustomClaimTypeConstants.OrganizationName, organizationName));
      }

      ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
      return new ClaimsPrincipal(identity);
    }
  }
}