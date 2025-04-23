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
    private readonly ExceptionService _exceptionService;

    public DiagnosticsApiController(RequestContext requestContext, RequestLogService requestLogService, ExceptionService exceptionService)
    {
      _requestLogService = new RequestLogService(requestContext.DatabaseName, requestContext.DatabasePassword);
      _exceptionService = new ExceptionService(requestContext.DatabaseName, requestContext.DatabasePassword);
    }

    [HttpGet("get-request-logs")]
    public async Task<IActionResult> GetRequestLogs(
      int page = 1,
      int pageSize = 10)
    {
      var (requestLogs, nextPage) = await _requestLogService.GetAllAsync(page, pageSize);

      RequestLogsPaginatedViewModel getRequestLogsViewModel = new RequestLogsPaginatedViewModel()
      {
        RequestLogs = requestLogs.Select(log => new RequestLogsPaginatedViewModel.RequestLogViewModel()
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

    [HttpGet("get-exceptions")]
    public async Task<IActionResult> GetExceptions(
      int page = 1,
      int pageSize = 10)
    {
      var (exceptions, nextPage) = await _exceptionService.GetAllAsync(page, pageSize);

      var getExceptionsViewModel = new ExceptionsPaginatedViewModel()
      {
        Exceptions = exceptions.Select(ex => new ExceptionsPaginatedViewModel.ExceptionViewModel()
        {
          RowNumber = ex.RowNumber,
          ExceptionID = ex.ExceptionID,
          Message = ex.Message,
          StackTrace = ex.StackTrace,
          Source = ex.Source,
          HResult = ex.HResult,
          TargetSite = ex.TargetSite,
          InnerException = ex.InnerException,
          RequestLogId = ex.RequestLogId,
          Created = ex.Created
        }).ToList(),
        Page = page,
        NextPage = nextPage
      };

      return Ok(getExceptionsViewModel);
    }
  }
}