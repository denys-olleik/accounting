using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class GeneralLedgerReconciliationTransactionService
  {
    public async Task<GeneralLedgerReconciliationTransaction> CreateAsync(GeneralLedgerReconciliationTransaction generalLedgerExpense)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetGeneralLedgerReconciliationTransactionManager().CreateAsync(generalLedgerExpense);
    }

    public async Task<List<GeneralLedgerReconciliationTransaction>> GetLastRelevantTransactionsAsync(int reconciliationTransactionId, int organizationId, bool loadChildren = false)
    {
      FactoryManager factoryManager = new FactoryManager();
      var manager = factoryManager.GetGeneralLedgerReconciliationTransactionManager();

      var transactions = await manager.GetLastTransactionAsync(reconciliationTransactionId, organizationId, loadChildren);
      var lastTransaction = transactions.OrderByDescending(t => t.GeneralLedgerReconciliationTransactionID).FirstOrDefault();

      if (lastTransaction != null)
      {
        if (lastTransaction.ReversedGeneralLedgerReconciliationTransactionId.HasValue)
        {
          // Last is a reversal, get all reversals
          return transactions.Where(t => t.ReversedGeneralLedgerReconciliationTransactionId.HasValue).ToList();
        }
        else
        {
          // Last is not a reversal, get all non-reversals
          return transactions.Where(t => !t.ReversedGeneralLedgerReconciliationTransactionId.HasValue).ToList();
        }
      }

      return new List<GeneralLedgerReconciliationTransaction>();
    }
  }
}