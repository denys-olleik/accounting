using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class TagService : BaseService
  {
    public TagService() : base()
    {
      
    }

    public TagService(
      string databaseName,
      string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<Tag> CreateAsync(Tag tag)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetTagManager().CreateAsync(tag);
    }

    public async Task<List<Tag>> GetAllAsync()
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetTagManager().GetAllAsync();
    }

    public async Task<Tag> GetByNameAsync(string name)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetTagManager().GetByNameAsync(name);
    }
  }
}