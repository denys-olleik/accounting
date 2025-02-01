using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class ItemService : BaseService
  {
    public ItemService() : base()
    {

    }

    public ItemService(
      string databaseName, 
      string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<Item> CreateAsync(Item item)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetItemManager().CreateAsync(item);
    }

    public async Task<int> DeleteAsync(int itemId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetItemManager().DeleteAsync(itemId);
    }

    public async Task<List<Item>> GetAllAsync(int page, int pageSize, int organizationId, int includeChildren)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetItemManager().GetAllAsync(organizationId);
    }

    public async Task<(List<Item> Items, int? NextPageNumber)> GetAllAsync(
      int page,
      int pageSize,
      int organizationId,
      bool includeDescendants,
      bool includeInventories)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetItemManager().GetAllAsync(
        page,
        pageSize,
        organizationId,
        includeDescendants,
        includeInventories);
    }

    public async Task<List<Item>> GetAllAsync(int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetItemManager().GetAllAsync(organizationId);
    }

    public async Task<Item> GetAsync(int itemId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetItemManager().GetAsync(itemId, organizationId);
    }

    public async Task<List<Item>?> GetChildrenAsync(int itemId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetItemManager().GetChildrenAsync(itemId, organizationId);
    }
  }
}