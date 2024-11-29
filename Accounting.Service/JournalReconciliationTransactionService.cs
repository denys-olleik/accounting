using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class JournalReconciliationTransactionService
  {
    private readonly string _databaseName;

    public JournalReconciliationTransactionService(string databaseName = DatabaseThing.DatabaseConstants.Database)
    {
      _databaseName = databaseName;
    }

    public async Task<JournalReconciliationTransaction> CreateAsync(JournalReconciliationTransaction journalExpense)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetJournalReconciliationTransactionManager().CreateAsync(journalExpense);
    }

    public async Task<List<JournalReconciliationTransaction>> GetLastRelevantTransactionsAsync(
      int reconciliationTransactionId,
      int organizationId,
      bool loadChildren = false)
    {
      var factoryManager = new FactoryManager(_databaseName);
      var manager = factoryManager.GetJournalReconciliationTransactionManager();

      var transactions = await manager.GetLastTransactionAsync(reconciliationTransactionId, organizationId, loadChildren);
      var lastTransaction = transactions.OrderByDescending(t => t.JournalReconciliationTransactionID).FirstOrDefault();

      if (lastTransaction != null)
      {
        if (lastTransaction.ReversedJournalReconciliationTransactionId.HasValue)
        {
          return transactions.Where(t => t.ReversedJournalReconciliationTransactionId.HasValue).ToList();
        }
        else
        {
          return transactions.Where(t => !t.ReversedJournalReconciliationTransactionId.HasValue).ToList();
        }
      }

      return new List<JournalReconciliationTransaction>();
    }
  }
}