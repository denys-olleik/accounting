namespace Accounting.Business
{
  public class GeneralLedgerTableBase
  {
    public int GeneralLedgerId { get; set; }
    public GeneralLedger? GeneralLedger { get; set; }
    public Guid TransactionGuid { get; set; }
    public DateTime Created { get; set; }
  }
}