using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class ToDoTagService : BaseService
  {
    public ToDoTagService() : base()
    {
      
    }

    public ToDoTagService(
      string databaseName,
      string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<ToDoTag> CreateAsync(ToDoTag taskTag)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetTaskTagManager().CreateAsync(taskTag);
    }
  }
}