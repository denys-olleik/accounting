using Accounting.CustomAttributes;
using Accounting.Models.Item;
using Accounting.Models.RequestLogViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("request-log")]
  public class RequestLogController : BaseController
  {
    [HttpGet]
    [Route("request-logs")]
    public IActionResult RequestLogs(int page = 1, int pageSize = 2)
    {
      var referer = Request.Headers["Referer"].ToString() ?? string.Empty;

      var vm = new RequestLogsPaginatedViewModel
      {
        Page = page,
        PageSize = pageSize,
        RememberPageSize = string.IsNullOrEmpty(referer),
      };

      return View(vm);
    }
  }
}