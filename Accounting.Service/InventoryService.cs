using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class InventoryService : BaseService
  {
    public InventoryService() : base()
    {
      
    }

    public InventoryService(string databaseName, string databasePassword) : base(databaseName, databasePassword)
    {

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