using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.LocationViewModels;
using Accounting.Service;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [ApiController]
  [Route("api/l")]
  public class LocationApiController : BaseController
  {
    private readonly LocationService _locationService;

    public LocationApiController(LocationService locationService, RequestContext requestContext)
    {
      _locationService = new LocationService(requestContext.DatabaseName, requestContext.DatabasePassword);
    }

    [HttpGet("locations")]
    public async Task<IActionResult> GetAllLocations(
      bool includeDescendants,
      bool includeInventories,
      int page = 1,
      int pageSize = 2)
    {
      (List<Location> locations, int? nextPage) =
        await _locationService.GetAllAsync(
          page,
          pageSize,
          GetOrganizationId(),
          includeDescendants,
          includeInventories);

      GetAllLocationsViewModel.LocationViewModel ConvertToViewModel(Location location)
      {
        var viewModel = new GetAllLocationsViewModel.LocationViewModel
        {
          LocationID = location.LocationID,
          Name = location.Name,
          Children = new List<GetAllLocationsViewModel.LocationViewModel>(),
          Inventories = location.Inventories?.Select(x => new GetAllLocationsViewModel.InventoryViewModel
          {
            InventoryID = x.InventoryID,
            ItemId = x.ItemId,
            LocationId = x.LocationId,
            Item = new GetAllLocationsViewModel.ItemViewModel
            {
              ItemID = x.Item.ItemID,
              Name = x.Item.Name
            },
            Quantity = x.Quantity,
            SellFor = x.SellFor
          }).ToList()
        };

        if (location.Children != null)
        {
          foreach (var child in location.Children)
          {
            viewModel.Children.Add(ConvertToViewModel(child));
          }
        }

        return viewModel;
      }

      return Ok(new GetAllLocationsViewModel
      {
        Locations = locations.Select(ConvertToViewModel).ToList(),
        Page = page,
        NextPage = nextPage,
        PageSize = pageSize
      });
    }

    private LocationViewModel ConvertToViewModel(Location location)
    {
      var viewModel = new LocationViewModel
      {
        LocationID = location.LocationID,
        Name = location.Name,
        Created = location.Created,
        ParentLocationId = location.ParentLocationId,
        CreatedById = location.CreatedById,
        Children = new List<LocationViewModel>()
      };

      if (location.Children != null)
      {
        foreach (var child in location.Children)
        {
          viewModel.Children.Add(ConvertToViewModel(child));
        }
      }

      return viewModel;
    }
  }
}