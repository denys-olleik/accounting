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
  }
}