using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using static Accounting.Business.Claim;

namespace Accounting.CustomAttributes
{
    public class AuthorizeWithOrganizationIdAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var organizationIdClaim = user.Claims.FirstOrDefault(c => c.Type == CustomClaimTypeConstants.OrganizationId);

            if (organizationIdClaim == null || string.IsNullOrEmpty(organizationIdClaim.Value))
            {
                context.Result = new RedirectToActionResult("ChooseOrganization", "Account", null);
                return;
            }
        }
    }
}