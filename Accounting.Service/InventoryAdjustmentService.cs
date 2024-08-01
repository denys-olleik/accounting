using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class InventoryAdjustmentService
  {
    public async Task<InventoryAdjustment> CreateAsync(InventoryAdjustment inventoryAdjustment)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetInventoryAdjustmentManager().CreateAsync(inventoryAdjustment);
    }
  }
}