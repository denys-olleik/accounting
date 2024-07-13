using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IToDoManager : IGenericRepository<ToDo, int>
  {
    Task<List<ToDo>> GetAllAsync(int organizationId);
    Task<ToDo> GetAsync(int id, int organizationId);
    Task<List<ToDo>> GetChildrenAsync(int parentId, int organizationId);
    Task<List<ToDo>> GetDescendantsAsync(int id, int organizationId);
    Task<List<ToDo>> GetToDoChildrenAsync(int id, int organizationId);
    Task<ToDo> UpdateContentAsync(int taskId, string content, int organizationId);
    Task<int> UpdateParentToDoIdAsync(int taskId, int? newParentId, int organizationId);
    Task<int> UpdateTaskStatusIdAsync(int taskId, string status, int organizationId);
  }
}