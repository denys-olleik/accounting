using Accounting.Business;
using Microsoft.AspNetCore.Authentication.Cookies;
using static Accounting.Business.Claim;
using System.Security.Claims;

namespace Accounting.Helpers
{
  public class AuthenticationHelper
  {
    public ClaimsPrincipal CreateClaimsPricipal(
      User user,
      int? organizationId = null,
      string? organizationName = null,
      string? databaseName = null)
    {
      List<System.Security.Claims.Claim> claims = new List<System.Security.Claims.Claim>();

      if (organizationId.HasValue && !string.IsNullOrEmpty(organizationName))
      {
        claims.Add(new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()));
        claims.Add(new System.Security.Claims.Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim()));
        claims.Add(new System.Security.Claims.Claim(ClaimTypes.Email, user.Email));
        claims.Add(new System.Security.Claims.Claim(CustomClaimTypeConstants.OrganizationId, organizationId.Value.ToString()));
        claims.Add(new System.Security.Claims.Claim(CustomClaimTypeConstants.OrganizationName, organizationName));
        if (!string.IsNullOrEmpty(databaseName))
        {
          claims.Add(new System.Security.Claims.Claim(CustomClaimTypeConstants.DatabaseName, databaseName));
        }
      }
      else
      {
        claims.Add(new System.Security.Claims.Claim(ClaimTypes.Email, user.Email));
      }

      claims.Add(new System.Security.Claims.Claim(CustomClaimTypeConstants.Password, user.Password));

      ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
      return new ClaimsPrincipal(identity);
    }
  }
}