using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.ItemViewModels;
using Accounting.Service;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [ApiController]
  [Route("api/inv")]
  public class ItemApiController : BaseController
  {
    private readonly ItemService _itemService;
    private readonly InventoryService _inventoryService;
    private readonly LocationService _locationService;

    public ItemApiController(
      ItemService itemService,
      InventoryService inventoryService,
      LocationService locationService)
    {
      _itemService = itemService;
      _inventoryService = inventoryService;
      _locationService = locationService;
    }

    [HttpGet("items")]
    public async Task<IActionResult> ItemsAndAssemblies(
      bool includeDescendants,
      int page = 1, 
      int pageSize = 2)
    {
      (List<Item> items, int? nextPageNumber) = 
        await _itemService.GetAllAsync(
          page, 
          pageSize,
          includeDescendants, 
          GetOrganizationId());

      ItemsAndAssembliesViewModel vm = new ItemsAndAssembliesViewModel
      {
        Items = items.Select(x => new ItemsAndAssembliesViewModel.ItemViewModel
        {
          ItemID = x.ItemID,
          Name = x.Name,
          Description = x.Description,
          Quantity = x.Quantity
        }).ToList(),
        NextPage = nextPageNumber,
        Page = page,
        PageSize = pageSize
      };

      return Ok(vm);
    }

    [HttpGet("inventories")]
    public async Task<IActionResult> Inventories(
      int page = 1,
      int pageSize = 10)
    {
      var (items, nextPageNumber) = await _itemService.GetAllAsync(
        page,
        pageSize,
        true,
        GetOrganizationId());

      foreach (var item in items)
      {
        item.Children = await _itemService.GetChildrenAsync(item.ItemID, GetOrganizationId());

        item.Inventories = await _inventoryService.GetAllAsync(
          new[] { item.ItemID }, GetOrganizationId());

        foreach (var inventory in item.Inventories!)
        {
          inventory.Location = await _locationService.GetAsync(inventory.LocationId, GetOrganizationId());
        }
      }

      var vm = new InventoriesViewModel
      {
        Items = items.Select(x => new InventoriesViewModel.ItemViewModel
        {
          ItemID = x.ItemID,
          Name = x.Name,
          Description = x.Description,
          Inventories = x.Inventories?.Select(y => new InventoriesViewModel.InventoryViewModel
          {
            InventoryID = y.InventoryID,
            ItemId = y.ItemId,
            LocationId = y.LocationId,
            Location = new InventoriesViewModel.LocationViewModel
            {
              LocationID = y.Location!.LocationID,
              Name = y.Location.Name
            },
            Quantity = y.Quantity,
            SalePrice = y.SalePrice
          }).ToList(),
          Children = x.Children?.Select(y => new InventoriesViewModel.ItemViewModel
          {
            ItemID = y.ItemID,
            Name = y.Name,
            Description = y.Description
          }).ToList(),
        }).ToList(),
        NextPage = nextPageNumber,
        CurrentPage = page
      };

      return Ok(vm);
    }
  }
}