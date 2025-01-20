using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class InventoryService
  {
    private readonly string _databaseName;
    private readonly string _databasePassword;

    public InventoryService(string databasePassword = "password", string databaseName = DatabaseThing.DatabaseConstants.Database)
    {
      _databaseName = databaseName;
      _databasePassword = databasePassword;
    }

    public async Task CreateAsync(Inventory inventory)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      await factoryManager.GetInventoryManager().CreateAsync(inventory);
    }

    public async Task<List<Inventory>?> GetAllAsync(int page, int pageSize, int organizationId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetInventoryManager().GetAllAsync(page, pageSize, organizationId);
    }

    public async Task<List<Inventory>?> GetAllAsync(int[] itemIds, int organizationId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetInventoryManager().GetAllAsync(itemIds, organizationId);
    }
  }
}