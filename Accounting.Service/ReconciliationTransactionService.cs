using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class ReconciliationTransactionService
  {
    public async Task<List<ReconciliationTransaction>> GetAllByReconciliationIdAsync(int id, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetReconciliationTransactionManager().GetAllByIdAsync(id, organizationId);
    }

    public async Task<ReconciliationTransaction> GetAsync(int reconciliationTransactionID)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetReconciliationTransactionManager().GetAsync(reconciliationTransactionID);
    }

    public async Task<(List<ReconciliationTransaction>, int? NextPageNumber)> GetReconciliationTransactionsAsync(
      int reconciliationId, 
      int page, 
      int pageSize)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetReconciliationTransactionManager().GetReconciliationTransactionAsync(reconciliationId, page, pageSize);
    }

    public async Task<int> ImportAsync(List<ReconciliationTransaction> reconciliationTransactions)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetReconciliationTransactionManager().ImportAsync(reconciliationTransactions);
    }

    public async Task UpdateAssetOrLiabilityAccountIdAsync(int reconciliationTransactionID, int selectedReconciliationLiabilitiesAndAssetsAccountId)
    {
      FactoryManager factoryManager = new FactoryManager();
      await factoryManager.GetReconciliationTransactionManager().UpdateAssetOrLiabilityAccountIdAsync(reconciliationTransactionID, selectedReconciliationLiabilitiesAndAssetsAccountId);
    }

    public async Task UpdateExpenseAccountIdAsync(int reconciliationTransactionID, int selectedReconciliationExpenseAccountId)
    {
      FactoryManager factoryManager = new FactoryManager();
      await factoryManager.GetReconciliationTransactionManager().UpdateExpenseAccountIdAsync(reconciliationTransactionID, selectedReconciliationExpenseAccountId);
    }

    public async Task<int> UpdateReconciliationTransactionInstructionAsync(int reconciliationTransactionID, string reconciliationInstruction)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetReconciliationTransactionManager().UpdateReconciliationTransactionInstructionAsync(reconciliationTransactionID, reconciliationInstruction);
    }
  }
}