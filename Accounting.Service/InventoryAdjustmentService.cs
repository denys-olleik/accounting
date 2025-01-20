using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class InventoryAdjustmentService
  {
    private readonly string _databaseName;
    private readonly string _databasePassword;

    public InventoryAdjustmentService(string databasePassword = "password", string databaseName = DatabaseThing.DatabaseConstants.Database)
    {
      _databaseName = databaseName;
      _databasePassword = databasePassword;
    }

    public async Task<InventoryAdjustment> CreateAsync(InventoryAdjustment inventoryAdjustment)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetInventoryAdjustmentManager().CreateAsync(inventoryAdjustment);
    }
  }
}