using Accounting.Business;
using Accounting.Common;
using Accounting.CustomAttributes;
using Accounting.Models.Item;
using Accounting.Models.ItemViewModels;
using Accounting.Service;
using Accounting.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;
using static Accounting.Business.Account;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("item")]
  public class ItemController : BaseController
  {
    private readonly AccountService _accountService;
    private readonly LocationService _locationService;
    private readonly InventoryService _inventoryService;
    private readonly ItemService _itemService;
    private readonly InventoryAdjustmentService _inventoryAdjustmentService;
    private readonly JournalInventoryAdjustmentService _journalInventoryAdjustmentService;
    private readonly JournalService _journalService;

    public ItemController(
      AccountService accountService,
      LocationService locationService,
      InventoryService inventoryService,
      ItemService itemService,
      InventoryAdjustmentService inventoryAdjustmentService,
      JournalInventoryAdjustmentService journalInventoryAdjustmentService,
      JournalService journalService)
    {
      _accountService = accountService;
      _locationService = locationService;
      _inventoryService = inventoryService;
      _itemService = itemService;
      _inventoryAdjustmentService = inventoryAdjustmentService;
      _journalInventoryAdjustmentService = journalInventoryAdjustmentService;
      _journalService = journalService;
    }

    [HttpGet]
    [Route("items")]
    public IActionResult Items(
      int page = 1,
      int pageSize = 2)
    {
      var vm = new ItemsPaginatedViewModel
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
        model.ParentItem = new ItemViewModel
        {
          ItemID = parentItem.ItemID,
          Name = parentItem.Name
        };
      }

      List<Account> accounts
        = await _accountService.GetAllAsync(AccountTypeConstants.Revenue, GetOrganizationId());
      accounts.AddRange(
        await _accountService.GetAllAsync(AccountTypeConstants.Assets, GetOrganizationId()));

      model.Accounts = new List<CreateItemViewModel.AccountViewModel>();
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

      foreach (Account account in accounts)
      {
        model.Accounts.Add(new CreateItemViewModel.AccountViewModel
        {
          AccountID = account.AccountID,
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
          model.ParentItem = new ItemViewModel
          {
            ItemID = parentItem.ItemID,
            Name = parentItem.Name
          };
        }

        List<Account> accounts
          = await _accountService.GetAllAsync(AccountTypeConstants.Revenue, GetOrganizationId());
        accounts.AddRange(
          await _accountService.GetAllAsync(AccountTypeConstants.Assets, GetOrganizationId()));

        model.Accounts = new List<CreateItemViewModel.AccountViewModel>();
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

        foreach (Account account in accounts)
        {
          model.Accounts.Add(new CreateItemViewModel.AccountViewModel
          {
            AccountID = account.AccountID,
            Name = account.Name,
            Type = account.Type
          });
        }

        model.SelectedItemType = model.SelectedItemType;

        return View(model);
      }

      Item item = new Item()
      {
        Name = model.Name,
        Description = model.Description,
        Quantity = model.Quantity,
        RevenueAccountId = model.SelectedRevenueAccountId,
        AssetsAccountId = model.SelectedAssetsAccountId,
        ItemType = model.SelectedItemType,
        InventoryMethod = model.SelectedInventoryMethod!,
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
            LocationId = model.SelectedLocationId.Value,
            Quantity = model.Quantity,
            SalePrice = model.SalePrice,
            CreatedById = GetUserId(),
            OrganizationId = GetOrganizationId()
          });

          await _inventoryAdjustmentService.CreateAsync(new InventoryAdjustment
          {
            ItemId = item.ItemID,
            ToLocationId = model.SelectedLocationId.Value,
            Quantity = model.Quantity,
            CreatedById = GetUserId(),
            OrganizationId = GetOrganizationId()
          });


        }

        scope.Complete();
      }

      return RedirectToAction("Items");
    }
  }

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

    [HttpGet("get-all-items")]
    public async Task<IActionResult> GetAllItems(
      bool includeDescendants,
      bool includeInventories,
      int page = 1,
      int pageSize = 2)
    {
      (List<Item> items, int? nextPage) =
        await _itemService.GetAllAsync(
          page,
          pageSize,
          GetOrganizationId(),
          includeDescendants,
          includeInventories);

      ItemViewModel ConvertToViewModel(Item item)
      {
        var viewModel = new ItemViewModel
        {
          ItemID = item.ItemID,
          Name = item.Name,
          Description = item.Description,
          Quantity = item.Quantity,
          UnitTypeId = item.UnitTypeId,
          ItemType = item.ItemType,
          InventoryMethod = item.InventoryMethod,
          RevenueAccountId = item.RevenueAccountId,
          AssetsAccountId = item.AssetsAccountId,
          ParentItemId = item.ParentItemId,
          Created = item.Created,
          CreatedById = item.CreatedById,
          OrganizationId = item.OrganizationId,
          Children = new List<ItemViewModel>()
        };

        if (item.Children != null)
        {
          foreach (var child in item.Children)
          {
            viewModel.Children.Add(ConvertToViewModel(child));
          }
        }

        return viewModel;
      }

      return Ok(new GetAllItemsViewModel
      {
        Items = items.Select(ConvertToViewModel).ToList(),
        Page = page,
        NextPage = nextPage,
        PageSize = pageSize
      });
    }

    [HttpGet("inventories")]
    public async Task<IActionResult> Inventories(
      int page = 1,
      int pageSize = 10)
    {
      var (items, nextPageNumber) = await _itemService.GetAllAsync(
        page,
        pageSize,
        GetOrganizationId(),
        true,
        false);

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
        Page = page
      };

      return Ok(vm);
    }
  }
}

namespace Accounting.Models.Item
{
  public class ItemViewModel
  {
    public int ItemID { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Quantity { get; set; }
    public int? UnitTypeId { get; set; }
    public string? ItemType { get; set; }
    public string? InventoryMethod { get; set; }
    public int? RevenueAccountId { get; set; }
    public int? AssetsAccountId { get; set; }
    public int? ParentItemId { get; set; }
    public DateTimeOffset Created { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }
    public List<ItemViewModel>? Children { get; internal set; }
    public List<Inventory>? Inventories { get; set; }
  }

  public class ItemsPaginatedViewModel : PaginatedViewModel
  {

  }

  public class GetAllItemsViewModel : PaginatedViewModel
  {
    public List<ItemViewModel>? Items { get; set; }
  }
}