using Accounting.Common;

namespace Accounting.Business
{
  public class ReconciliationExpenseCategory : IIdentifiable<int>
  {
    public int ReconciliationExpenseCategoryID { get; set; }
    public string? Name { get; set; }
    public DateTime Created { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }

    public int Identifiable => this.ReconciliationExpenseCategoryID;
  }
}