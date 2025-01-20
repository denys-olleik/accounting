using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class RequestLogService
  {
    private readonly string _databaseName;
    private readonly string _databasePassword;

    public RequestLogService(string databasePassword = "password", string databaseName = DatabaseThing.DatabaseConstants.Database)
    {
      _databaseName = databaseName;
      _databasePassword = databasePassword;
    }

    public async Task<RequestLog> CreateAsync(RequestLog requestLog)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetRequestLogManager().CreateAsync(requestLog);
    }
  }
}