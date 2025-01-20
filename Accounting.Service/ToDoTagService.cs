using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class ToDoTagService
  {
    private readonly string _databaseName;
    private readonly string _databasePassword;

    public ToDoTagService(string databasePassword = "password", string databaseName = DatabaseThing.DatabaseConstants.Database)
    {
      _databaseName = databaseName;
      _databasePassword = databasePassword;
    }

    public async Task<ToDoTag> CreateAsync(ToDoTag taskTag)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetTaskTagManager().CreateAsync(taskTag);
    }
  }
}