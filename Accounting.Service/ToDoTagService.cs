using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class ToDoTagService
  {
    public async Task<ToDoTag> CreateAsync(ToDoTag taskTag)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetTaskTagManager().CreateAsync(taskTag);
    }
  }
}