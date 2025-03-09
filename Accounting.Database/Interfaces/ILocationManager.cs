using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface ILocationManager : IGenericRepository<Location, int>
  {
    Task<int> DeleteAsync(int locationID, bool deleteChildren);
    Task<List<Location>> GetAllAsync(int organizationId);
    Task<(List<Location> locations, int? nextPage)> GetAllAsync(
      int page, 
      int pageSize, 
      int organizationId, 
      bool includeDescendants, 
      bool includeInventories);
    Task<Location?> GetAsync(int locationId, int organizationId);
    Task<List<Location>?> GetChildrenAsync(int locationId, int organizationId);
    Task<bool> IsLocationInUseAsync(int locationId, int organizationId);
    Task<int> UpdateAsync(int locationId, string? name);
  }
}