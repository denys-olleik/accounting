using Accounting.Common;

namespace Accounting.Business
{
  public class InventoryAdjustment : IIdentifiable<int>
  {
    public int InventoryAdjustmentID { get; set; }
    public int ItemId { get; set; }
    public int? ToLocationId { get; set; }
    public int? FromLocationId { get; set; }
    public decimal Quantity { get; set; }
    public DateTime Created { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }
    public Item? Item { get; set; }
    public Location? ToLocation { get; set; }
    public Location? FromLocation { get; set; }
    public int Identifiable => this.InventoryAdjustmentID;
  }
}