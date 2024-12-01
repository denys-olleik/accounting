using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class InventoryAdjustmentService
  {
    private readonly string _databaseName;

    public InventoryAdjustmentService(RequestContext requestContext)
    {
      _databaseName = requestContext.DatabaseName;
    }

    public async Task<InventoryAdjustment> CreateAsync(InventoryAdjustment inventoryAdjustment)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInventoryAdjustmentManager().CreateAsync(inventoryAdjustment);
    }
  }
}