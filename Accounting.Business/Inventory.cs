using Accounting.Common;

namespace Accounting.Business
{
  public class Inventory : IIdentifiable<int>
  {
    public int InventoryID { get; set; }
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    public int LocationId { get; set; }
    public Location? Location { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? SellFor { get; set; }
    public DateTime Created { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }
    public int Identifiable => this.InventoryID;
  }
}