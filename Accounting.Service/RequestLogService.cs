using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class RequestLogService
  {
    public async Task<RequestLog> CreateAsync(RequestLog requestLog)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetRequestLogManager().CreateAsync(requestLog);
    }
  }
}