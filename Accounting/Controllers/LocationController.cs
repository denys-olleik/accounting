using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.InvoiceViewModels;
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
      _locationService = new LocationService(requestContext.DatabaseName);
    }

    [Route("locations")]
    public IActionResult Locations()
    { 
      return View();
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