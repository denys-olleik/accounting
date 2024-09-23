namespace Accounting.Business
{
  public class JournalTableBase
  {
    public int JournalId { get; set; }
    public Journal? Journal { get; set; }
    public Guid TransactionGuid { get; set; }
    public DateTime Created { get; set; }
  }
}