using Accounting.Common;

namespace Accounting.Business
{
  public class Organization : IIdentifiable<int>
  {
    public int OrganizationID { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? AccountsReceivableEmail { get; set; }
    public string? AccountsPayableEmail { get; set; }
    public string? AccountsReceivablePhone { get; set; }
    public string? AccountsPayablePhone { get; set; }
    public string? Website { get; set; }
    public string? PaymentInstructions { get; set; }
    public int TenantId { get; set; }
    public DateTime Created { get; set; }

    public int Identifiable => this.OrganizationID;
  }
}