using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class InventoryService
  {
    public async Task CreateAsync(Inventory inventory)
    {
      FactoryManager factoryManager = new FactoryManager();
      await factoryManager.GetInventoryManager().CreateAsync(inventory);
    }

    public async Task<List<Inventory>?> GetAllAsync(int page, int pageSize, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetInventoryManager().GetAllAsync(page, pageSize, organizationId);
    }

    public async Task<List<Inventory>?> GetAllAsync(int[] itemIds, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetInventoryManager().GetAllAsync(itemIds, organizationId);
    }
  }
}