using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class JournalReconciliationTransactionService
  {
    public async Task<JournalReconciliationTransaction> CreateAsync(JournalReconciliationTransaction journalExpense)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetJournalReconciliationTransactionManager().CreateAsync(journalExpense);
    }

    public async Task<List<JournalReconciliationTransaction>> GetLastRelevantTransactionsAsync(int reconciliationTransactionId, int organizationId, bool loadChildren = false)
    {
      FactoryManager factoryManager = new FactoryManager();
      var manager = factoryManager.GetJournalReconciliationTransactionManager();

      var transactions = await manager.GetLastTransactionAsync(reconciliationTransactionId, organizationId, loadChildren);
      var lastTransaction = transactions.OrderByDescending(t => t.JournalReconciliationTransactionID).FirstOrDefault();

      if (lastTransaction != null)
      {
        if (lastTransaction.ReversedJournalReconciliationTransactionId.HasValue)
        {
          // Last is a reversal, get all reversals
          return transactions.Where(t => t.ReversedJournalReconciliationTransactionId.HasValue).ToList();
        }
        else
        {
          // Last is not a reversal, get all non-reversals
          return transactions.Where(t => !t.ReversedJournalReconciliationTransactionId.HasValue).ToList();
        }
      }

      return new List<JournalReconciliationTransaction>();
    }
  }
}