using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class TagService
  {
    public async Task<Tag> CreateAsync(Tag tag)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetTagManager().CreateAsync(tag);
    }

    public async Task<List<Tag>> GetAllAsync()
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetTagManager().GetAllAsync();
    }

    public async Task<Tag> GetByNameAsync(string name)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetTagManager().GetByNameAsync(name);
    }
  }
}