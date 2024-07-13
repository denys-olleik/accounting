using Accounting.Common;

namespace Accounting.Business
{
  public class ReconciliationExpense : IIdentifiable<int>
  {
    public int ReconciliationExpenseID { get; set; }
    public decimal? Amount { get; set; }
    public int ExpenseChartOfAccountId { get; set; }
    public int AssetOrLiabilityChartOfAccountId { get; set; }
    public int ReconciliationTransactionId { get; set; }
    public DateTime Created { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }

    public int Identifiable => this.ReconciliationExpenseID;
  }
}