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

      var databaseExists = await TenantExistsAsync(databaseName);
      if (!databaseExists)
      {
        Console.WriteLine("Database does not exist.");
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

        user = userOrganization.User!;
        organization = userOrganization.Organization!;
      }
      else
      {
        UserService userService = new ();
        var (existingUser, _) = await userService.GetFirstOfAnyTenantAsync(email);
        user = existingUser;
      }

      if (user == null || user.Password != password)
      {
        context.RejectPrincipal();
        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return;
      }
    }

    private async Task<bool> TenantExistsAsync(string? databaseName)
    {
      TenantService tenantService = new();
      return await tenantService.TenantExistsAsync(databaseName);
    }
  }
}