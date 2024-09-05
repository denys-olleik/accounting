using FluentValidation.Results;

namespace Accounting.Models.ItemViewModels
{
  public class CreateItemViewModel
  {
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? ParentItemId { get; set; }
    public ItemViewModel? ParentItem { get; set; }
    public int? SelectedAssetsAccountId { get; set; }
    public int? SelectedRevenueAccountId { get; set; }

    #region Inventory
    public decimal SalePrice { get; set; }
    public decimal InitialCost { get; set; }
    public decimal Quantity { get; set; }

    public List<string> AvailableItemTypes { get; set; } = new List<string>();
    public string? SelectedItemType { get; set; }

    public List<LocationViewModel>? Locations { get; set; }
    public int? SelectedLocationId { get; set; }
    #endregion

    public List<AccountViewModel>? Accounts { get; set; }
    public List<string> AvailableInventoryMethods { get; set; } = new List<string>();
    public string? SelectedInventoryMethod { get; set; }

    public ValidationResult? ValidationResult { get; set; }

    public class LocationViewModel
    {
      public int LocationID { get; set; }
      public string? Name { get; set; }
      public string? Description { get; set; }
    }

    public class AccountViewModel
    {
      public int AccountID { get; set; }
      public string? Name { get; set; }
      public string? Type { get; set; }
    }

    public class ItemViewModel
    {
      public int ItemID { get; set; }
      public string? Name { get; set; }
    }
  }
}