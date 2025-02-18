namespace Accounting.Models.LocationViewModels
{
  public class GetAllLocationsViewModel
  {
    public List<LocationViewModel>? Locations { get; set; }
    public int? Page { get; set; }
    public int? NextPage { get; set; }
    public int PageSize { get; internal set; }

    public class LocationViewModel
    {
      public int LocationID { get; set; }
      public string? Name { get; set; }
      public string? Description { get; set; }
      public DateTime Created { get; set; }
      public int ParentLocationId { get; set; }
      public int CreatedById { get; set; }
      public int OrganizationId { get; set; }
      public List<LocationViewModel>? Children { get; set; }
      public List<InventoryViewModel>? Inventories { get; set; }
    }

    public class InventoryViewModel
    {
      public int InventoryID { get; set; }
      public int ItemId { get; set; }
      public int LocationId { get; set; }
      public ItemViewModel Item { get; set; }
      public decimal? Quantity { get; set; }
      public decimal? SellFor { get; set; }
    }

    public class ItemViewModel
    {
      public int ItemID { get; set; }
      public string? Name { get; set; }
    }
  }
}