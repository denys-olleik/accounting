using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Accounting.Helpers
{
  public class AuthenticationHelper
  {
    public static ClaimsPrincipal CreateClaimsPrincipal(
      Business.User user,
      int? organizationId = null,
      string? organizationName = null,
      string? databaseName = null,
      string? databasePassword = null)
    {
      List<Claim> claims = new List<Claim>();

      if (organizationId.HasValue && !string.IsNullOrEmpty(organizationName))
      {
        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()));
        claims.Add(new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim()));
        claims.Add(new Claim(ClaimTypes.Email, user.Email));
        claims.Add(new Claim(Business.Claim.CustomClaimTypeConstants.OrganizationId, organizationId.Value.ToString()));
        claims.Add(new Claim(Business.Claim.CustomClaimTypeConstants.OrganizationName, organizationName));
        if (!string.IsNullOrEmpty(databaseName))
        {
          claims.Add(new Claim(Business.Claim.CustomClaimTypeConstants.DatabaseName, databaseName));
        }
        if (!string.IsNullOrEmpty(databasePassword))
        {
          claims.Add(new Claim(Business.Claim.CustomClaimTypeConstants.DatabasePassword, databasePassword));
        }
      }
      else
      {
        claims.Add(new Claim(ClaimTypes.Email, user.Email));
      }

      claims.Add(new Claim(Business.Claim.CustomClaimTypeConstants.Password, user.Password));

      ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
      return new ClaimsPrincipal(identity);
    }
  }
}