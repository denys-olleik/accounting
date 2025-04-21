using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class RequestLogService : BaseService
  {
    public RequestLogService() : base()
    {
      
    }

    public RequestLogService(
      string databaseName,
      string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<RequestLog> CreateAsync(RequestLog requestLog)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetRequestLogManager().CreateAsync(requestLog);
    }

    public async Task<(IEnumerable<RequestLog> requestLogs, int? nextPage)> GetAllAsync(int page, int pageSize)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetRequestLogManager().GetAllAsync(page, pageSize);
    }

    public async Task<int> UpdateResponseAsync(int requestLogId, string statusCode, long responseLength)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      var requestLog = await factoryManager.GetRequestLogManager().GetByIdAsync(requestLogId);
      if (requestLog != null)
      {
        requestLog.StatusCode = statusCode;
        requestLog.ResponseLengthBytes = responseLength;
        return await factoryManager.GetRequestLogManager().UpdateResponseAsync(requestLog.RequestLogID, statusCode, responseLength);
      }
      else
      {
        throw new System.Exception("Request log not found");
      }
    }
  }
}