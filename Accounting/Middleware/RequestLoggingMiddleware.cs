using Accounting.Business;
using Accounting.Service;

namespace Accounting.Middleware
{
  public class RequestLoggingMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly RequestLogService _requestLogService;

    public RequestLoggingMiddleware(RequestDelegate next, RequestLogService requestLogService)
    {
      _next = next;
      _requestLogService = requestLogService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      var originalResponseBodyStream = context.Response.Body;

      using (var responseBody = new MemoryStream())
      {
        context.Response.Body = responseBody;

        await _next(context);

        context.Response.ContentLength = responseBody.Length;

        var remoteIp = context.Request.Headers["X-Real-IP"].FirstOrDefault() ??
                       context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ??
                       context.Connection.RemoteIpAddress?.ToString();

        var requestLog = new RequestLog
        {
          RemoteIp = remoteIp,
          CountryCode = "",
          Referer = context.Request.Headers["Referer"],
          UserAgent = context.Request.Headers["User-Agent"],
          Path = context.Request.Path,
          StatusCode = context.Response.StatusCode.ToString(),
          ResponseLengthBytes = responseBody.Length
        };

        responseBody.Position = 0;
        await responseBody.CopyToAsync(originalResponseBodyStream);

        await _requestLogService.CreateAsync(requestLog);
      }

      context.Response.Body = originalResponseBodyStream;
    }
  }
}