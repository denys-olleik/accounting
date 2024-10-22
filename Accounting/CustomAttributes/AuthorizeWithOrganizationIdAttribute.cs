using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using static Accounting.Business.Claim;

namespace Accounting.CustomAttributes
{
  public class AuthorizeWithOrganizationIdAttribute : AuthorizeAttribute, IAuthorizationFilter
  {
    public void OnAuthorization(AuthorizationFilterContext context)
    {
      // Check if the action or controller has the AllowAnonymous attribute
      var hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata
          .Any(em => em is IAllowAnonymous);

      if (hasAllowAnonymous)
      {
        // If AllowAnonymous is detected, skip the authorization check
        return;
      }

      var user = context.HttpContext.User;
      if (user == null || !user.Identity.IsAuthenticated)
      {
        context.Result = new UnauthorizedResult();
        return;
      }

      var organizationIdClaim = user.Claims.FirstOrDefault(c => c.Type == CustomClaimTypeConstants.OrganizationId);

      if (organizationIdClaim == null || string.IsNullOrEmpty(organizationIdClaim.Value))
      {
        context.Result = new RedirectToActionResult("ChooseOrganization", "UserAccount", null);
        return;
      }
    }
  }
}