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

    [HttpGet("locations")]
    public async Task<IActionResult> Locations(
      int page = 1,
      int pageSize = 2)
    {
      var referer = Request.Headers["Referer"].ToString() ?? string.Empty;

      var vm = new LocationsPaginatedViewModel
      {
        Page = page,
        PageSize = pageSize,
        RememberPageSize = string.IsNullOrEmpty(referer)
      };

      return View(vm);
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

    [HttpGet]
    [Route("delete/{locationId}")]
    public async Task<IActionResult> Delete(int locationId)
    {
      Location location = await _locationService.GetAsync(locationId, GetOrganizationId());

      if (location == null)
        return NotFound();

      bool isLocationInUseAsync = await _locationService.IsLocationInUseAsync(locationId, GetOrganizationId());
      location.Children = await _locationService.GetChildrenAsync(locationId, GetOrganizationId());

      return View(new DeleteLocationViewModel
      {
        LocationID = location.LocationID,
        Name = location.Name,
        IsLocationInUseAsync = isLocationInUseAsync
      });
    }

    [HttpPost]
    [Route("delete/{locationId}")]
    public async Task<IActionResult> Delete(DeleteLocationViewModel model)
    {
      Location location = await _locationService.GetAsync(model.LocationID, GetOrganizationId());

      if (location == null)
        return NotFound();

      bool isLocationInUseAsync = await _locationService.IsLocationInUseAsync(model.LocationID, GetOrganizationId());
      location.Children = await _locationService.GetChildrenAsync(model.LocationID, GetOrganizationId());
      model.IsLocationInUseAsync = isLocationInUseAsync;

      DeleteLocationViewModel.DeleteLocationViewModelValidator validator = new DeleteLocationViewModel.DeleteLocationViewModelValidator();
      ValidationResult validationResult = await validator.ValidateAsync(model);

      location.Children = await _locationService.GetChildrenAsync(location.LocationID, GetOrganizationId());
      model.IsLocationInUseAsync = location.Children?.Count > 0;

      if (!validationResult.IsValid)
      {
        model.LocationID = location.LocationID;
        model.Name = location.Name;
        model.ValidationResult = validationResult;
        return View(model);
      }

      try
      {
        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
          await _locationService.DeleteAsync(model.LocationID, model.DeleteChildren);
          scope.Complete();
        }
      }
      catch (InvalidOperationException ex)
      {
        if (ex.Message.Contains("23503"))
        {
          model.ValidationResult.Errors.Add(new ValidationFailure(nameof(model.LocationID), ex.Message));
          return View(model);
        }
        throw;
      }

      return RedirectToAction("Locations");
    }
  }
}