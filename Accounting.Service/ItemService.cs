using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class ItemService
  {
    public async Task<Item> CreateAsync(Item item)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetItemManager().CreateAsync(item);
    }

    public async Task<List<Item>> GetAllAsync(int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetItemManager().GetAllAsync(organizationId);
    }

    public async Task<(List<Item> Items, int? NextPageNumber)> GetAllAsync(
      int page, 
      int pageSize, 
      bool loadChildren,
      int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetItemManager()
        .GetAllAsync(
          page, 
          pageSize,
          loadChildren,
          organizationId);
    }

    public async Task<Item> GetAsync(int itemId, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetItemManager().GetAsync(itemId, organizationId);
    }

    public async Task<List<Item>?> GetChildrenAsync(int itemId, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetItemManager().GetChildrenAsync(itemId, organizationId);
    }
  }
}