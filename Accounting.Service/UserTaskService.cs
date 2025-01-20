using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class UserTaskService
  {
    private readonly string _databaseName;
    private readonly string _databasePassword;

    public UserTaskService(string databasePassword = "password", string databaseName = DatabaseThing.DatabaseConstants.Database)
    {
      _databaseName = databaseName;
      _databasePassword = databasePassword;
    }

    public async Task<UserToDo> CreateAsync(UserToDo userTask)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetUserTaskManager().CreateAsync(userTask);
    }

    public async Task<List<User>> GetUsers(int taskId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetUserTaskManager().GetUsersAsync(taskId, organizationId);
    }
  }
}
