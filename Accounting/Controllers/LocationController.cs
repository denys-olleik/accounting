using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.InvoiceViewModels;
using Accounting.Models.Item;
using Accounting.Models.LocationViewModels;
using Accounting.Service;
using Accounting.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("l")]
  public class LocationController : BaseController
  {
    private readonly LocationService _locationService;

    public LocationController(RequestContext requestContext)
    {
      _locationService = new LocationService(requestContext.DatabaseName, requestContext.DatabasePassword);
    }

    [HttpGet("get-all-locations")]
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

    [Route("create/{parentLocationId?}")]
    [HttpGet]
    public async Task<IActionResult> Create(int? parentLocationId)
    {
      Location? parentLocation = null;

      if (parentLocationId.HasValue)
      {
        parentLocation = await _locationService.GetAsync(parentLocationId.Value, GetOrganizationId());
        if (parentLocation == null)
          return NotFound();
      }

      var model = new CreateLocationViewModel();

      if (parentLocation != null)
      {
        model.ParentLocationId = parentLocationId;
        model.ParentLocation = new CreateLocationViewModel.LocationViewModel()
        {
          LocationId = parentLocation!.LocationID,
          Name = parentLocation!.Name
        };
      }

      return View(model);
    }

    [HttpPost]
    [Route("create/{parentLocationId?}")]
    public async Task<IActionResult> Create(CreateLocationViewModel model)
    {
      CreateLocationViewModelValidator validator = new CreateLocationViewModelValidator();
      ValidationResult result = await validator.ValidateAsync(model);

      if (!result.IsValid)
      {
        model.ValidationResult = result;
        return View(model);
      }

      using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        Location location = new Location
        {
          Name = model.Name,
          Description = model.Description,
          ParentLocationId = model.ParentLocationId,
          CreatedById = GetUserId(),
          OrganizationId = GetOrganizationId()
        };

        await _locationService.CreateLocationAsync(location);

        scope.Complete();
      }

      return RedirectToAction("Locations");
    }
  }
}