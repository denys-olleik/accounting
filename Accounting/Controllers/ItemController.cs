using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.ItemViewModels;
using Accounting.Service;
using Accounting.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;
using static Accounting.Business.ChartOfAccount;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("item")]
  public class ItemController : BaseController
  {
    private readonly ChartOfAccountService _chartOfAccountService;
    private readonly LocationService _locationService;
    private readonly InventoryService _inventoryService;
    private readonly ItemService _itemService;
    private readonly InventoryLedgerService _inventoryLedgerService;

    public ItemController(
      ChartOfAccountService chartOfAccountService, 
      LocationService locationService, 
      InventoryService inventoryService, 
      ItemService itemService)
    {
      _chartOfAccountService = chartOfAccountService;
      _locationService = locationService;
      _inventoryService = inventoryService;
      _itemService = itemService;
    }

    [HttpGet]
    [Route("products-and-services")]
    public async Task<IActionResult> ProductsAndServices(int page = 1, int pageSize = 10)
    {
      var vm = new ProductsAndServicesPaginatedViewModel
      {
        Page = page,
        PageSize = pageSize
      };

      return View(vm);
    }
    
    [HttpGet]
    [Route("create/{parentItemId?}")]
    public async Task<IActionResult> Create(int? parentItemId)
    {
      CreateItemViewModel model = new CreateItemViewModel();

      if (parentItemId != null)
      {
        Item parentItem = await _itemService.GetAsync(parentItemId.Value, GetOrganizationId());
        if (parentItem == null)
          return NotFound();

        model.ParentItemId = parentItemId;
        model.ParentItem = new CreateItemViewModel.ItemViewModel
        {
          ItemID = parentItem.ItemID,
          Name = parentItem.Name
        };
      }

      List<ChartOfAccount> accounts
        = await _chartOfAccountService.GetAllAsync(AccountTypeConstants.Revenue, GetOrganizationId());
      accounts.AddRange(
        await _chartOfAccountService.GetAllAsync(AccountTypeConstants.Assets, GetOrganizationId()));

      model.Accounts = new List<CreateItemViewModel.ChartOfAccountViewModel>();
      model.AvailableInventoryMethods = Item.InventoryMethods.All.ToList();
      model.AvailableItemTypes = Item.ItemTypes.All.ToList();
      model.Locations = new List<CreateItemViewModel.LocationViewModel>();

      List<Location> locations = await _locationService.GetAllAsync(GetOrganizationId());

      foreach (var location in locations)
      {
        model.Locations.Add(new CreateItemViewModel.LocationViewModel
        {
          LocationID = location.LocationID,
          Name = location.Name
        });
      }

      foreach (ChartOfAccount account in accounts)
      {
        model.Accounts.Add(new CreateItemViewModel.ChartOfAccountViewModel
        {
          ChartOfAccountID = account.ChartOfAccountID,
          Name = account.Name,
          Type = account.Type
        });
      }

      return View(model);
    }

    [HttpPost]
    [Route("create/{parentItemId?}")]
    public async Task<IActionResult> Create(CreateItemViewModel model)
    {
      CreateItemViewModelValidator validator = new CreateItemViewModelValidator(GetOrganizationId());
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;

        if (model.ParentItemId != null)
        {
          Item parentItem = await _itemService.GetAsync(model.ParentItemId.Value, GetOrganizationId());
          if (parentItem == null)
            return NotFound();

          model.ParentItemId = parentItem.ItemID;
          model.ParentItem = new CreateItemViewModel.ItemViewModel
          {
            ItemID = parentItem.ItemID,
            Name = parentItem.Name
          };
        }

        List<ChartOfAccount> accounts
        = await _chartOfAccountService.GetAllAsync(AccountTypeConstants.Revenue, GetOrganizationId());
        accounts.AddRange(
          await _chartOfAccountService.GetAllAsync(AccountTypeConstants.Assets, GetOrganizationId()));

        model.Accounts = new List<CreateItemViewModel.ChartOfAccountViewModel>();
        model.AvailableInventoryMethods = Item.InventoryMethods.All.ToList();
        model.AvailableItemTypes = Item.ItemTypes.All.ToList();
        model.Locations = new List<CreateItemViewModel.LocationViewModel>();

        List<Location> locations = await _locationService.GetAllAsync(GetOrganizationId());

        foreach (var location in locations)
        {
          model.Locations.Add(new CreateItemViewModel.LocationViewModel
          {
            LocationID = location.LocationID,
            Name = location.Name
          });
        }

        foreach (ChartOfAccount account in accounts)
        {
          model.Accounts.Add(new CreateItemViewModel.ChartOfAccountViewModel
          {
            ChartOfAccountID = account.ChartOfAccountID,
            Name = account.Name,
            Type = account.Type
          });
        }

        model.SelectedItemType = model.SelectedItemType;

        return View(model);
      }

      Item item = new Item
      {
        Name = model.Name,
        Description = model.Description,
        RevenueChartOfAccountId = model.SelectedRevenueChartOfAccountId!.Value,
        AssetsChartOfAccountId = model.SelectedAssetsChartOfAccountId!.Value,
        ItemType = model.SelectedItemType,
        InventoryMethod = model.SelectedInventoryMethod,
        ParentItemId = model.ParentItemId,
        CreatedById = GetUserId(),
        OrganizationId = GetOrganizationId()
      };

      using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        item = await _itemService.CreateAsync(item);

        if (model.SelectedLocationId > 0)
        {
          await _inventoryService.CreateAsync(new Inventory
          {
            ItemId = item.ItemID,
            LocationId = model.SelectedLocationId,
            Quantity = model.Quantity,
            SalePrice = model.SalePrice,
            CreatedById = GetUserId(),
            OrganizationId = GetOrganizationId()
          });

          await _inventoryLedgerService.CreateAsync(new InventoryLedger
          {
            ItemId = item.ItemID,
            LocationToId = model.SelectedLocationId,
            LocationFromId = null,
            Quantity = model.Quantity,
            CreatedById = GetUserId(),
            OrganizationId = GetOrganizationId()
          });
        }

        scope.Complete();
      }

      return RedirectToAction("ProductsAndServices");
    }
  }
}