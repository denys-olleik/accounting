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
    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
      var principal = context.Principal;

      int? userId = null;

      var nameIdentifierClaim = principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
      if (nameIdentifierClaim != null && !string.IsNullOrEmpty(nameIdentifierClaim.Value) && int.TryParse(nameIdentifierClaim.Value, out int parsedValue))
      {
        userId = parsedValue;
      }
      string email = principal!.Claims.Single(x => x.Type == ClaimTypes.Email).Value;

      int organizationId = Convert.ToInt32(principal.Claims.SingleOrDefault(x => x.Type == CustomClaimTypeConstants.OrganizationId)?.Value);

      string password = principal.Claims.Single(x => x.Type == CustomClaimTypeConstants.Password).Value;

      UserService userService = new UserService();
      UserOrganizationService userOrganizationService = new UserOrganizationService();

      User user;

      if (organizationId > 0)
      {
        var userOrganization = await userOrganizationService.GetAsync(userId!.Value, organizationId);
        user = userOrganization.User!;
      }
      else
      {
        user = await userService.GetAsync(email, true);
      }

      if (user == null || user.Password != password)
      {
        context.RejectPrincipal();

        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await base.ValidatePrincipal(context);
      }
      else
      {
        context.Principal = principal;
        await base.ValidatePrincipal(context);
      }
    }
  }
}