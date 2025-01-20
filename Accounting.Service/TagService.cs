using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class TagService
  {
    private readonly string _databaseName;
    private readonly string _databasePassword;

    public TagService(string databasePassword = "password", string databaseName = DatabaseThing.DatabaseConstants.Database)
    {
      _databaseName = databaseName;
      _databasePassword = databasePassword;
    }

    public async Task<Tag> CreateAsync(Tag tag)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetTagManager().CreateAsync(tag);
    }

    public async Task<List<Tag>> GetAllAsync()
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetTagManager().GetAllAsync();
    }

    public async Task<Tag> GetByNameAsync(string name)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetTagManager().GetByNameAsync(name);
    }
  }
}
