using Accounting.Common;

namespace Accounting.Business
{
  public class InventoryLedger : IIdentifiable<int>
  {
    public int InventoryLedgerID { get; set; }
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    public int? LocationToId { get; set; }
    public Location? LocationTo { get; set; }
    public int? LocationFromId { get; set; }
    public Location? LocationFrom { get; set; }
    public decimal? Quantity { get; set; }
    public DateTime Created { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }

    public int Identifiable => this.InventoryLedgerID;
  }
}