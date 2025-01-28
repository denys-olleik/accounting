using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class JournalReconciliationTransactionService : BaseService
  {
    public JournalReconciliationTransactionService() : base()
    {

    }

    public JournalReconciliationTransactionService(
      string databaseName,
      string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<JournalReconciliationTransaction> CreateAsync(JournalReconciliationTransaction journalExpense)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetJournalReconciliationTransactionManager().CreateAsync(journalExpense);
    }

    public async Task<List<JournalReconciliationTransaction>> GetLastRelevantTransactionsAsync(
      int reconciliationTransactionId,
      int organizationId,
      bool loadChildren = false)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
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