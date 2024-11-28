using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class InventoryService
  {
    private readonly string _databaseName;

    public InventoryService(string databaseName)
    {
      _databaseName = databaseName;
    }

    public async Task CreateAsync(Inventory inventory)
    {
      var factoryManager = new FactoryManager(_databaseName);
      await factoryManager.GetInventoryManager().CreateAsync(inventory);
    }

    public async Task<List<Inventory>?> GetAllAsync(int page, int pageSize, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInventoryManager().GetAllAsync(page, pageSize, organizationId);
    }

    public async Task<List<Inventory>?> GetAllAsync(int[] itemIds, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInventoryManager().GetAllAsync(itemIds, organizationId);
    }
  }
}