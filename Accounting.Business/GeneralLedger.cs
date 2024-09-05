using Accounting.Common;

namespace Accounting.Business
{
  public class GeneralLedger : IIdentifiable<int>
  {
    public int GeneralLedgerID { get; set; }
    public int AccountId { get; set; }
    public Account? ChartOfAccount { get; set; }
    public decimal? Debit { get; set; }
    public decimal? Credit { get; set; }
    public string? Memo { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }
    public DateTime Created { get; set; }

    public int Identifiable => this.GeneralLedgerID;
  }
}