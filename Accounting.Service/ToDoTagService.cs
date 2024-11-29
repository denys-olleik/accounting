using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class ToDoTagService
  {
    private readonly string _databaseName;

    public ToDoTagService(string databaseName = DatabaseThing.DatabaseConstants.Database)
    {
      _databaseName = databaseName;
    }

    public async Task<ToDoTag> CreateAsync(ToDoTag taskTag)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetTaskTagManager().CreateAsync(taskTag);
    }
  }
}