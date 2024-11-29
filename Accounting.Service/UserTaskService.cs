using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class UserTaskService
  {
    private readonly string _databaseName;

    public UserTaskService(string databaseName = DatabaseThing.DatabaseConstants.Database)
    {
      _databaseName = databaseName;
    }

    public async Task<UserToDo> CreateAsync(UserToDo userTask)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetUserTaskManager().CreateAsync(userTask);
    }

    public async Task<List<User>> GetUsers(int taskId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetUserTaskManager().GetUsersAsync(taskId, organizationId);
    }
  }
}
