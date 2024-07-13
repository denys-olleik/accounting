using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IInventoryManager : IGenericRepository<Inventory, int>
  {
    Task<List<Inventory>?> GetAllAsync(int page, int pageSize, int organizationId);
    Task<List<Inventory>?> GetAllAsync(int[] itemIds, int organizationId);
  }
}