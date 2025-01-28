using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class UserTaskService : BaseService
  {
    public UserTaskService() : base()
    {

    }

    public UserTaskService(
      string databaseName,
      string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<UserToDo> CreateAsync(UserToDo userTask)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetUserTaskManager().CreateAsync(userTask);
    }

    public async Task<List<User>> GetUsers(int taskId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetUserTaskManager().GetUsersAsync(taskId, organizationId);
    }
  }
}