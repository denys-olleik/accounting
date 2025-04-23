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

    public async Task<(IEnumerable<Business.Exception> exceptions, int? nextPage)> GetAllAsync(
      int page,
      int pageSize)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetExceptionManager().GetAllAsync(page, pageSize);
    }
  }
}