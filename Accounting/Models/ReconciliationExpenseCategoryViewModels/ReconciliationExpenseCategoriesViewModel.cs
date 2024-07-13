namespace Accounting.Models.ReconciliationExpenseCategoryViewmodels
{
  public class ReconciliationExpenseCategoriesViewModel
  {
    public List<ReconciliationExpenseCategoryViewModel>? ReconciliationExpenseCategories { get; set; }

    public class ReconciliationExpenseCategoryViewModel
    {
      public int ReconciliationExpenseCategoryID { get; set; }
      public string? Name { get; set; }
      public DateTime? Created { get; set; }
      public int? CreatedById { get; set; }
      public int? OrganizationId { get; set; }
    }
  }
}