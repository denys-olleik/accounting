using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IItemManager : IGenericRepository<Item, int>
  {
    Task<List<Item>> GetAllAsync(int organizationId);
    Task<(List<Item> Items, int? NextPageNumber)> GetAllAsync(int page, int pageSize, bool includeChildren, int organizationId);
    Task<Item> GetAsync(int itemId, int organizationId);
    Task<List<Item>?> GetChildrenAsync(int itemId, int organizationId);
  }
}