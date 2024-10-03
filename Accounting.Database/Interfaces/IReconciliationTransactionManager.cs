using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IReconciliationTransactionManager : IGenericRepository<ReconciliationTransaction, int>
  {
    Task<List<ReconciliationTransaction>> GetAllByIdAsync(int reconciliationId, int organizationId);
    Task<ReconciliationTransaction> GetAsync(int reconciliationTransactionID);
    Task<(List<ReconciliationTransaction> reconciliationTransactions, int? nextPage)> GetReconciliationTransactionAsync(int reconciliationId, int page, int pageSize);
    Task<int> ImportAsync(List<ReconciliationTransaction> reconciliationTransactions);
    Task<int> UpdateAssetOrLiabilityAccountIdAsync(int reconciliationTransactionID, int selectedReconciliationLiabilitiesAndAssetsAccountId);
    Task<int> UpdateExpenseAccountIdAsync(int reconciliationTransactionID, int selectedReconciliationExpenseAccountId);
    Task<int> UpdateReconciliationTransactionInstructionAsync(int reconciliationTransactionID, string reconciliationInstructionJSON);
  }
}