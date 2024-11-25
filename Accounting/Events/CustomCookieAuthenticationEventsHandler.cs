using Accounting.Business;
using Accounting.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using static Accounting.Business.Claim;

namespace Accounting.Events
{
  public class CustomCookieAuthenticationEventsHandler : CookieAuthenticationEvents
  {
    private readonly UserService _userService;
    private readonly UserOrganizationService _userOrganizationService;

    public CustomCookieAuthenticationEventsHandler(UserService userService, UserOrganizationService userOrganizationService)
    {
      _userService = userService;
      _userOrganizationService = userOrganizationService;
    }

    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
      var principal = context.Principal;

      int? userId = null;
      string? databaseName = null;

      var nameIdentifierClaim = principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
      if (nameIdentifierClaim != null && int.TryParse(nameIdentifierClaim.Value, out int parsedValue))
      {
        userId = parsedValue;
      }

      string email = principal?.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
      var databaseNameClaim = principal?.Claims.FirstOrDefault(x => x.Type == CustomClaimTypeConstants.DatabaseName);
      if (databaseNameClaim != null)
      {
        databaseName = databaseNameClaim.Value;
      }

      int organizationId = Convert.ToInt32(principal?.Claims.SingleOrDefault(x => x.Type == CustomClaimTypeConstants.OrganizationId)?.Value);
      string password = principal?.Claims.SingleOrDefault(x => x.Type == CustomClaimTypeConstants.Password)?.Value;

      User user;
      Organization organization;

      if (organizationId > 0)
      {
        var userOrganization = await _userOrganizationService.GetAsync(userId!.Value, organizationId, databaseName!);
        user = userOrganization.User!;
        organization = userOrganization.Organization!;
      }
      else
      {
        var (existingUser, _) = await _userService.GetFirstOfAnyTenantAsync(email);
        user = existingUser;
      }

      if (user == null || user.Password != password)
      {
        context.RejectPrincipal();
        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
      }
      else
      {
        var claimsPrincipal = CreateClaimsPrincipal(user, organizationId, databaseName);
        await context.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties { IsPersistent = true });
      }
    }

    private ClaimsPrincipal CreateClaimsPrincipal(User user, int? organizationId = null, string? databaseName = null)
    {
      var claims = new List<System.Security.Claims.Claim>
        {
            new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
            new System.Security.Claims.Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim()),
            new System.Security.Claims.Claim(ClaimTypes.Email, user.Email)
        };

      if (organizationId.HasValue)
      {
        claims.Add(new System.Security.Claims.Claim(CustomClaimTypeConstants.OrganizationId, organizationId.Value.ToString()));
      }

      if (!string.IsNullOrEmpty(databaseName))
      {
        claims.Add(new System.Security.Claims.Claim(CustomClaimTypeConstants.DatabaseName, databaseName));
      }

      claims.Add(new System.Security.Claims.Claim(CustomClaimTypeConstants.Password, user.Password));

      var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
      return new ClaimsPrincipal(identity);
    }
  }
}