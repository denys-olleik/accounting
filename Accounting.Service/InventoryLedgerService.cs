using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class InventoryLedgerService
  {
    public async Task<InventoryLedger> CreateAsync(InventoryLedger inventoryLedger)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetInventoryLedgerManager().CreateAsync(inventoryLedger);
    }
  }
}