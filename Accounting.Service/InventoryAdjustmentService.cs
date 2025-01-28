using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class InventoryAdjustmentService : BaseService
  {
    public InventoryAdjustmentService() : base()
    {
      
    }

    public InventoryAdjustmentService(
      string databaseName, 
      string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<InventoryAdjustment> CreateAsync(InventoryAdjustment inventoryAdjustment)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetInventoryAdjustmentManager().CreateAsync(inventoryAdjustment);
    }
  }
}