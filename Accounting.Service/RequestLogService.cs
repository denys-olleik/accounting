using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class RequestLogService
  {
    private readonly string _databaseName;

    public RequestLogService(string databaseName)
    {
      _databaseName = databaseName;
    }

    public async Task<RequestLog> CreateAsync(RequestLog requestLog)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetRequestLogManager().CreateAsync(requestLog);
    }
  }
}