using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class TagService
  {
    private readonly string _databaseName;

    public TagService(string databaseName)
    {
      _databaseName = databaseName;
    }

    public async Task<Tag> CreateAsync(Tag tag)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetTagManager().CreateAsync(tag);
    }

    public async Task<List<Tag>> GetAllAsync()
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetTagManager().GetAllAsync();
    }

    public async Task<Tag> GetByNameAsync(string name)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetTagManager().GetByNameAsync(name);
    }
  }
}
