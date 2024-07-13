namespace Accounting.Models.ReconciliationViewModels
{
  public class UpdateReconciliationTransactionInstructionViewModel
  {
    public int ReconciliationTransactionID { get; set; }
    public string? ReconciliationInstruction { get; set; }
    public int SelectedReconciliationExpenseAccountId { get; set; }
    public int SelectedReconciliationLiabilitiesAndAssetsAccountId { get; set; }
  }
}