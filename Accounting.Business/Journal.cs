using Accounting.Common;

namespace Accounting.Business
{
  public class Journal : IIdentifiable<int>
  {
    public int JournalID { get; set; }
    public int AccountId { get; set; }
    public Account? Account { get; set; }
    public decimal? Debit { get; set; }
    public decimal? Credit { get; set; }
    public string? Memo { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }
    public DateTime Created { get; set; }

    public int Identifiable => this.JournalID;
  }
}