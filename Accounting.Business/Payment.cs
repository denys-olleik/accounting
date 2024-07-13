using Accounting.Common;

namespace Accounting.Business
{
  public class Payment : IIdentifiable<int>
  {
    public int PaymentID { get; set; }
    public string? ReferenceNumber { get; set; }
    public decimal Amount { get; set; }
    public string? VoidReason { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }
    public DateTime Created { get; set; }

    public int Identifiable => this.PaymentID;
  }
}