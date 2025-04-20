using Accounting.Database;

namespace Accounting.Service
{
  public class ExceptionService : BaseService
  {
    public ExceptionService() : base()
    {

    }

    public ExceptionService(
      string databaseName,
      string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<Business.Exception> CreateAsync(Business.Exception exceptionLog)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetExceptionManager().CreateAsync(exceptionLog);
    }
  }
}