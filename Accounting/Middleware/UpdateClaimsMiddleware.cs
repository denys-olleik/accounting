using Accounting.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using static Accounting.Business.Claim;
using System.Security.Claims;

public class UpdateClaimsMiddleware
{
  private readonly RequestDelegate _next;
  private readonly UserOrganizationService _userOrganizationService;

  public UpdateClaimsMiddleware(RequestDelegate next, UserService userService, UserOrganizationService userOrganizationService)
  {
    _next = next;
    _userOrganizationService = userOrganizationService;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    if (context.User?.Identity?.IsAuthenticated ?? false)
    {
      var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var orgIdClaim = context.User.FindFirst(CustomClaimTypeConstants.OrganizationId)?.Value;
      var databaseName = context.User.FindFirst(CustomClaimTypeConstants.DatabaseName)?.Value;

      if (int.TryParse(userIdClaim, out int userId) && int.TryParse(orgIdClaim, out int orgId))
      {
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