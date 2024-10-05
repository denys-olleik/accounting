namespace Accounting.Models.ItemViewModels
{
  public class InventoriesViewModel
  {
    public List<ItemViewModel>? Items { get; set; }
    public int? Page { get; set; }
    public int? NextPage { get; set; }

    public class ItemViewModel
    {
      public int ItemID { get; set; }
      public string? Name { get; set; }
      public string? Description { get; set; }

      public List<ItemViewModel>? Children { get; set; }
      public List<InventoryViewModel>? Inventories { get; set; }
    }

    public class InventoryViewModel
    {
      public int InventoryID { get; set; }
      public int ItemId { get; set; }
      public int LocationId { get; set; }
      public LocationViewModel? Location { get; set; }
      public decimal? Quantity { get; set; }
      public decimal? SellFor { get; set; }
    }

    public class LocationViewModel
    {
      public int LocationID { get; set; }
      public string? Name { get; set; }
    }
  }
}