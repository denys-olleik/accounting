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

      int userId = Convert.ToInt32(principal!.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
      int organizationId = Convert.ToInt32(principal.Claims.SingleOrDefault(x => x.Type == CustomClaimTypeConstants.OrganizationId)?.Value);

      string password = principal.Claims.Single(x => x.Type == CustomClaimTypeConstants.Password).Value;

      UserService userService = new UserService();
      UserOrganizationService userOrganizationService = new UserOrganizationService();

      User user;

      if (organizationId > 0)
      { // TODO: Certify that userOrganization.User is not null, it probably can be.
        var userOrganization = await userOrganizationService.GetAsync(userId, organizationId);
        user = userOrganization.User!;
      }
      else
      {
        user = await userService.GetAsync(userId);
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