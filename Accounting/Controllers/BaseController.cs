using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using static Accounting.Business.Claim;

namespace Accounting.Controllers
{
  public class BaseController : Controller
  {
    [NonAction]
    public int GetUserId()
    {
      var identity = User?.Identity as ClaimsIdentity;
      if (identity == null)
      {
        throw new InvalidOperationException("User identity is not available.");
      }

      var userIdClaim = identity.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
      if (userIdClaim == null)
      {
        throw new InvalidOperationException("User identifier claim is not available.");
      }

      return Convert.ToInt32(userIdClaim.Value);
    }

    [NonAction]
    public int GetOrganizationId()
    {
      var identity = User?.Identity as ClaimsIdentity;
      if (identity == null)
      {
        throw new InvalidOperationException("User identity is not available.");
      }

      var organizationIdClaim = identity.Claims.SingleOrDefault(x => x.Type == CustomClaimTypeConstants.OrganizationId);
      if (organizationIdClaim == null)
      {
        throw new InvalidOperationException("Organization identifier claim is not available.");
      }

      return Convert.ToInt32(organizationIdClaim.Value);
    }

    [NonAction]
    public string GetBaseUrl()
    {
      var request = HttpContext.Request;
      return $"{request.Scheme}://{request.Host}";
    }

    [NonAction]
    public string GetEmail()
    {
      var identity = User?.Identity as ClaimsIdentity;
      if (identity == null)
      {
        throw new InvalidOperationException("User identity is not available.");
      }

      var emailClaim = identity.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Email);
      if (emailClaim == null || string.IsNullOrEmpty(emailClaim.Value))
      {
        throw new InvalidOperationException("Email claim is not available or invalid.");
      }

      return emailClaim.Value;
    }

    [NonAction]
    public string GetDatabaseName()
    {
      var identity = User?.Identity as ClaimsIdentity;
      if (identity == null)
      {
        throw new InvalidOperationException("User identity is not available.");
      }

      var databaseNameClaim = identity.Claims.SingleOrDefault(x => x.Type == CustomClaimTypeConstants.DatabaseName);
      if (databaseNameClaim == null || string.IsNullOrEmpty(databaseNameClaim.Value))
      {
        throw new InvalidOperationException("Database name claim is not available or invalid.");
      }

      return databaseNameClaim.Value;
    }

    [NonAction]
    public string? GetReferrerUrl()
    {
      return HttpContext.Request.Headers["Referer"].ToString();
    }
  }
}