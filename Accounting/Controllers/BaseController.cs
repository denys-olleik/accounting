using Accounting.Service;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Accounting.Business.Claim;

namespace Accounting.Controllers
{
  public class BaseController : Controller
  {
    [NonAction]
    public int GetUserId()
    {
      ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
      int userId = Convert.ToInt32(identity.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
      return userId;
    }

    [NonAction]
    public int GetOrganizationId()
    {
      ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
      int organizationId = Convert.ToInt32(identity.Claims.SingleOrDefault(x => x.Type == CustomClaimTypeConstants.OrganizationId)?.Value);
      return organizationId;
    }

    [NonAction]
    public string GetBaseUrl()
    {
      var request = HttpContext.Request;
      return $"{request.Scheme}://{request.Host}";
    }
  }
}