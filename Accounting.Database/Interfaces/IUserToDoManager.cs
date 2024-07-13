using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IUserToDoManager : IGenericRepository<UserToDo, int>
  {
    Task<List<User>> GetUsersAsync(int taskId, int organizationId);
  }
}