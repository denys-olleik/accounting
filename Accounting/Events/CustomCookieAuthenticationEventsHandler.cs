using Accounting.Business;
using Accounting.Common;
using Accounting.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using static Accounting.Business.Claim;

namespace Accounting.Events
{
  public class CustomCookieAuthenticationEventsHandler : CookieAuthenticationEvents
  {
    public CustomCookieAuthenticationEventsHandler()
    {

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

      List<string> roleClaims = principal?.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();

      var databaseExists = await TenantExistsAsync(databaseName);
      if (!databaseExists)
      {
        context.RejectPrincipal();
        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return;
      }

      int organizationId = Convert.ToInt32(principal?.Claims.SingleOrDefault(x => x.Type == CustomClaimTypeConstants.OrganizationId)?.Value);
      string password = principal?.Claims.SingleOrDefault(x => x.Type == CustomClaimTypeConstants.Password)?.Value;
      string databasePassword = principal?.Claims.SingleOrDefault(x => x.Type == CustomClaimTypeConstants.DatabasePassword)?.Value;

      User user;
      Organization organization = null;

      if (organizationId > 0)
      {
        UserOrganizationService userOrganizationService = new UserOrganizationService(databaseName, databasePassword);
        var userOrganization = await userOrganizationService.GetAsync(userId!.Value, organizationId);

        if (userOrganization == null)
        {
          context.RejectPrincipal();
          await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
          return;
        }

        await CompareAuthorizationClaimsExcept(
          context,
          userOrganization,
          roleClaims,
          databaseName,
          databasePassword,
          new List<string>
          {
            ConfigurationSingleton.ConfigurationConstants.TenantManagement
          });

        user = userOrganization.User!;
        organization = userOrganization.Organization!;
      }
      else
      {
        UserService userService = new();
        var (existingUser, _) = await userService.GetFirstOfAnyTenantAsync(email);
        user = existingUser;
      }

      if (user == null || user.Email != email)
      {
        context.RejectPrincipal();
        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return;
      }

      if (user == null || user.Password != password)
      {
        context.RejectPrincipal();
        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return;
      }
    }

    private async Task CompareAuthorizationClaimsExcept(
      CookieValidatePrincipalContext context,
      UserOrganization userOrganization,
      List<string>? roleClaims,
      string databaseName,
      string databasePassword,
      List<string>? roleClaimsToNotCompare = null)
    {
      ClaimService claimService = new(databaseName, databasePassword);

      List<string>? cookiesRoles = context.Principal.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();
      cookiesRoles?.RemoveAll(x => roleClaimsToNotCompare?.Contains(x) ?? false);

      List<string>? databaseRoles = await claimService.GetUserRolesAsync(userOrganization.UserId, userOrganization.OrganizationId, CustomClaimTypeConstants.Role);

      if (cookiesRoles != null && databaseRoles != null)
      {
        if (!cookiesRoles.OrderBy(x => x).SequenceEqual(databaseRoles.OrderBy(x => x)))
        {
          context.RejectPrincipal();
          await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
          return;
        }
      }
    }

    private async Task<bool> TenantExistsAsync(string? databaseName)
    {
      TenantService tenantService = new();
      return await tenantService.TenantExistsAsync(databaseName);
    }
  }
}