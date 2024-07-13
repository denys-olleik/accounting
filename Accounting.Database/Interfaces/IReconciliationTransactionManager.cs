using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IReconciliationTransactionManager : IGenericRepository<ReconciliationTransaction, int>
  {
    Task<List<ReconciliationTransaction>> GetAllByIdAsync(int reconciliationId, int organizationId);
    Task<ReconciliationTransaction> GetAsync(int reconciliationTransactionID);
    Task<(List<ReconciliationTransaction> ReconciliationTransactions, int? NextPageNumber)> GetReconciliationTransactionAsync(int reconciliationId, int page, int pageSize);
    Task<int> ImportAsync(List<ReconciliationTransaction> reconciliationTransactions);
    Task<int> UpdateAssetOrLiabilityChartOfAccountIdAsync(int reconciliationTransactionID, int selectedReconciliationLiabilitiesAndAssetsAccountId);
    Task<int> UpdateExpenseChartOfAccountIdAsync(int reconciliationTransactionID, int selectedReconciliationExpenseAccountId);
    Task<int> UpdateReconciliationTransactionInstructionAsync(int reconciliationTransactionID, string reconciliationInstructionJSON);
  }
}