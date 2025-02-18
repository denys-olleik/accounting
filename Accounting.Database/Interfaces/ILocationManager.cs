using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface ILocationManager : IGenericRepository<Location, int>
  {
    Task<(List<Location> Locations, int? NextPageNumber)> GetAllAsync(int page, int pageSize, int organizationId);
    Task<List<Location>> GetAllAsync(int organizationId);
    Task<(List<Location> locations, int? nextPage)> GetAllAsync(
      int page, 
      int pageSize, 
      int organizationId, 
      bool includeDescendants, 
      bool includeInventories);
    Task<Location?> GetAsync(int locationId, int organizationId);
  }
}