using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class ToDoService : BaseService
  {
    public ToDoService() : base()
    {
      
    }

    public ToDoService(
      string databaseName,
      string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<ToDo> CreateAsync(ToDo taskItem)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetTaskManager().CreateAsync(taskItem);
    }

    public async Task<List<ToDo>> GetAllAsync(int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      List<ToDo> taskItems = await factoryManager.GetTaskManager().GetAllAsync(organizationId);

      List<ToDo> rootTasks = taskItems.Where(t => t.ParentToDoId == null).ToList();

      foreach (ToDo rootTask in rootTasks)
      {
        BuildTree(taskItems, rootTask);
      }

      return rootTasks;
    }

    public async Task<List<ToDo>> GetChildrenAsync(int parentId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetTaskManager().GetChildrenAsync(parentId, organizationId);
    }

    private void BuildTree(List<ToDo> allTasks, ToDo parentTask)
    {
      List<ToDo> children = allTasks.Where(t => t.ParentToDoId == parentTask.ToDoID).ToList();

      foreach (ToDo child in children)
      {
        BuildTree(allTasks, child);
        parentTask.Children.Add(child);
      }
    }

    public async Task<ToDo> GetAsync(int id, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetTaskManager().GetAsync(id, organizationId);
    }

    public async Task<ToDo> UpdateContentAsync(int taskId, string content, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetTaskManager().UpdateContentAsync(taskId, content, organizationId);
    }

    public async Task<int> UpdateParentTaskIdAsync(int taskId, int? newParentId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetTaskManager().UpdateParentToDoIdAsync(taskId, newParentId, organizationId);
    }

    public async Task<List<ToDo>> GetTaskChildren(int id, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      ToDo rootTask = await GetAsync(id, organizationId);
      List<ToDo> descendants = await factoryManager.GetTaskManager().GetDescendantsAsync(id, organizationId);
      BuildTree(descendants, rootTask);
      return rootTask.Children;
    }

    public async Task<int> UpdateTaskStatusIdAsync(int taskId, string status, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetTaskManager().UpdateTaskStatusIdAsync(taskId, status, organizationId);
    }
  }
}
