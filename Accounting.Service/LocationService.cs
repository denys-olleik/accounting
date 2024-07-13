using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class LocationService
  {
    public async Task<Location> CreateLocationAsync(Location location)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetLocationService().CreateAsync(location);
    }

    public async Task<(List<Location> locations, int? nextPageNumber)> GetAllAsync(
      int page, 
      int pageSize, 
      int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetLocationService().GetAllAsync(page, pageSize, organizationId);
    }

    public async Task<List<Location>> GetAllHierarchicalAsync(int organizationId)
    {
      var allOrganizationLocationsFlatList = await GetAllAsync(organizationId);
      var rootLocations = allOrganizationLocationsFlatList.Where(x => x.ParentLocationId == null).ToList();

      foreach (var location in rootLocations)
      {
        location.Children = allOrganizationLocationsFlatList.Where(x => x.ParentLocationId == location.LocationID).ToList();

        if (location.Children.Any())
        {
          PopulateChildrenRecursively(location.Children, allOrganizationLocationsFlatList);
        }
      }

      return rootLocations;
    }

    public async Task<Location?> GetAsync(int locationId, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetLocationService().GetAsync(locationId, organizationId);
    }

    public async Task<List<Location>> GetAllAsync(int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetLocationService().GetAllAsync(organizationId);
    }

    private void PopulateChildrenRecursively(List<Location> children, List<Location> allLocations)
    {
      foreach (var child in children)
      {
        child.Children = allLocations.Where(x => x.ParentLocationId == child.LocationID).ToList();

        if (child.Children.Any())
        {
          PopulateChildrenRecursively(child.Children, allLocations);
        }
      }
    }
  }
}