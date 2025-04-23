using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.DiagnosticsViewModels;
using Accounting.Service;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("diagnostics")]
  public class DiagnosticsController : BaseController
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

    [HttpGet]
    [Route("exceptions")]
    public IActionResult Exceptions(int page = 1, int pageSize = 2)
    {
      var referer = Request.Headers["Referer"].ToString() ?? string.Empty;
      var vm = new ExceptionsPaginatedViewModel
      {
        Page = page,
        PageSize = pageSize,
        RememberPageSize = string.IsNullOrEmpty(referer),
      };
      return View(vm);
    }
  }

  [AuthorizeWithOrganizationId]
  [ApiController]
  [Route("api/diagnostics")]
  public class DiagnosticsApiController : BaseController
  {
    private readonly RequestLogService _requestLogService;

    public DiagnosticsApiController(RequestContext requestContext, RequestLogService requestLogService)
    {
      _requestLogService = new RequestLogService(requestContext.DatabaseName, requestContext.DatabasePassword);
    }

    [HttpGet("get-request-logs")]
    public async Task<IActionResult> GetRequestLogs(
      int page = 1,
      int pageSize = 10)
    {
      var (requestLogs, nextPage) = await _requestLogService.GetAllAsync(page, pageSize);

      Models.DiagnosticsViewModels.GetRequestLogsViewModel getRequestLogsViewModel = new Models.DiagnosticsViewModels.GetRequestLogsViewModel()
      {
        RequestLogs = requestLogs.Select(log => new Models.DiagnosticsViewModels.GetRequestLogsViewModel.RequestLogViewModel()
        {
          RowNumber = log.RowNumber,
          RequestLogID = log.RequestLogID,
          RemoteIp = log.RemoteIp,
          CountryCode = log.CountryCode,
          Referer = log.Referer,
          UserAgent = log.UserAgent,
          Path = log.Path,
          ResponseLengthBytes = log.ResponseLengthBytes,
          StatusCode = log.StatusCode,
          Created = log.Created
        }).ToList(),
        Page = page,
        NextPage = nextPage
      };

      return Ok(getRequestLogsViewModel);
    }
  }
}