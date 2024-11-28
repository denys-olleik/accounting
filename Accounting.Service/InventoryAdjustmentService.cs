using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class InventoryAdjustmentService
  {
    private readonly string _databaseName;

    public InventoryAdjustmentService(string databaseName)
    {
      _databaseName = databaseName;
    }

    public async Task<InventoryAdjustment> CreateAsync(InventoryAdjustment inventoryAdjustment)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInventoryAdjustmentManager().CreateAsync(inventoryAdjustment);
    }
  }
}