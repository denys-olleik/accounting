using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class ReconciliationTransactionService : BaseService
  {
    public ReconciliationTransactionService() : base()
    {
      
    }

    public ReconciliationTransactionService(
      string databaseName,
      string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<List<ReconciliationTransaction>> GetAllByReconciliationIdAsync(int id, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetReconciliationTransactionManager().GetAllByIdAsync(id, organizationId);
    }

    public async Task<ReconciliationTransaction> GetAsync(int reconciliationTransactionID)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetReconciliationTransactionManager().GetAsync(reconciliationTransactionID);
    }

    public async Task<(List<ReconciliationTransaction>, int? NextPageNumber)> GetReconciliationTransactionsAsync(
        int reconciliationId,
        int page,
        int pageSize)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetReconciliationTransactionManager().GetReconciliationTransactionAsync(reconciliationId, page, pageSize);
    }

    public async Task<int> ImportAsync(List<ReconciliationTransaction> reconciliationTransactions)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetReconciliationTransactionManager().ImportAsync(reconciliationTransactions);
    }

    public async Task UpdateAssetOrLiabilityAccountIdAsync(int reconciliationTransactionID, int selectedReconciliationLiabilitiesAndAssetsAccountId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      await factoryManager.GetReconciliationTransactionManager().UpdateAssetOrLiabilityAccountIdAsync(reconciliationTransactionID, selectedReconciliationLiabilitiesAndAssetsAccountId);
    }

    public async Task UpdateExpenseAccountIdAsync(int reconciliationTransactionID, int selectedReconciliationExpenseAccountId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      await factoryManager.GetReconciliationTransactionManager().UpdateExpenseAccountIdAsync(reconciliationTransactionID, selectedReconciliationExpenseAccountId);
    }

    public async Task<int> UpdateReconciliationTransactionInstructionAsync(int reconciliationTransactionID, string reconciliationInstruction)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetReconciliationTransactionManager().UpdateReconciliationTransactionInstructionAsync(reconciliationTransactionID, reconciliationInstruction);
    }
  }
}