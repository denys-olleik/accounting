using Accounting.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using static Accounting.Business.Claim;
using System.Security.Claims;

public class UpdateOrganizationNameClaimMiddleware
{
  private readonly RequestDelegate _next;

  public UpdateOrganizationNameClaimMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    if (context.User?.Identity?.IsAuthenticated ?? false)
    {
      var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var orgIdClaim = context.User.FindFirst(CustomClaimTypeConstants.OrganizationId)?.Value;
      var databaseName = context.User.FindFirst(CustomClaimTypeConstants.DatabaseName)?.Value;
      var databasePassword = context.User.FindFirst(CustomClaimTypeConstants.DatabasePassword)?.Value;

      if (int.TryParse(userIdClaim, out int userId) && int.TryParse(orgIdClaim, out int orgId))
      {
        UserOrganizationService _userOrganizationService = new (databaseName, databasePassword);
        var userOrganization = await _userOrganizationService.GetAsync(userId, orgId);
        if (userOrganization?.Organization != null)
        {
          var claims = new List<Claim>(context.User.Claims);
          claims.RemoveAll(c => c.Type == CustomClaimTypeConstants.OrganizationName);
          claims.Add(new Claim(CustomClaimTypeConstants.OrganizationName, userOrganization.Organization.Name!));

          var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
          context.User = new ClaimsPrincipal(identity);
        }
      }
    }

    await _next(context);
  }
}