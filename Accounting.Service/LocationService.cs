﻿using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class LocationService : BaseService
  {
    public LocationService() : base()
    {
      
    }

    public LocationService(
      string databaseName, 
      string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<Location> CreateLocationAsync(Location location)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetLocationService().CreateAsync(location);
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
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetLocationService().GetAsync(locationId, organizationId);
    }

    public async Task<List<Location>> GetAllAsync(int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
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

    public async Task<(List<Location> locations, int? nextPage)> GetAllAsync(
      int page, 
      int pageSize, 
      int organizationId, 
      bool includeDescendants, 
      bool includeInventories)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetLocationService().GetAllAsync(page, pageSize, organizationId, includeDescendants, includeInventories);
    }

    public async Task<int> DeleteAsync(int locationID, bool deleteChildren)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetLocationService().DeleteAsync(locationID, deleteChildren);
    }

    public async Task<List<Location>?> GetChildrenAsync(int locationId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetLocationService().GetChildrenAsync(locationId, organizationId);
    }

    public async Task<bool> IsLocationInUseAsync(int locationId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetLocationService().IsLocationInUseAsync(locationId, organizationId);
    }

    public async Task UpdateAsync(int locationId, string? name)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      await factoryManager.GetLocationService().UpdateAsync(locationId, name);
    }
  }
}