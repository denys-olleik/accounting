using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface ILocationManager : IGenericRepository<Location, int>
  {
    Task<(List<Location> Locations, int? NextPageNumber)> GetAllAsync(int page, int pageSize, int organizationId);
    Task<List<Location>> GetAllAsync(int organizationId);
    Task<Location?> GetAsync(int locationId, int organizationId);
  }
}