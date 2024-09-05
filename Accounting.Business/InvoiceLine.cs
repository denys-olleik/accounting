using Accounting.Common;

namespace Accounting.Business
{
  public class InvoiceLine : IIdentifiable<int>
  {
    public int InvoiceLineID { get; set; }

    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool TitleOrDescriptionModified { get; set; }

    public decimal? Quantity { get; set; }
    public decimal? Price { get; set; }
    public decimal? Received { get; set; }
    public bool QuantityOrPriceModified { get; set; }

    public int InvoiceId { get; set; }
    public DateTime Created { get; set; }
    public int RevenueAccountId { get; set; }
    public int AssetsAccountId { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }

    public int Identifiable => this.InvoiceLineID;
  }
}