namespace Accounting.Models.ReconciliationViewModels
{
  public class GetReconciliationTransactionsViewModel : PaginatedViewModel
  {
    public List<ReconciliationTransactionViewModel>? ReconciliationTransactions { get; set; }
   
    public class ReconciliationTransactionViewModel
    {
      public int ReconciliationTransactionID { get; set; }
      public string? Status { get; set; }
      public string? RawData { get; set; }
      public string? ReconciliationInstruction { get; set; }
      public DateTime? TransactionDate { get; set; }
      public DateTime? PostedDate { get; set; }
      public string? Description { get; set; }
      public decimal? Amount { get; set; }
      public string? Category { get; set; }
      public DateTime? Created { get; set; }
      public int? ReconciliationId { get; set; }
      public int? CreatedById { get; set; }
      public int? OrganizationId { get; set; }
    }
  }
}