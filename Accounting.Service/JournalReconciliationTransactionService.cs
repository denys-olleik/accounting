using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class JournalReconciliationTransactionService
  {
    private readonly string _databaseName;
    private readonly string _databasePassword;

    public JournalReconciliationTransactionService(string databasePassword = "password", string databaseName = DatabaseThing.DatabaseConstants.Database)
    {
      _databaseName = databaseName;
      _databasePassword = databasePassword;
    }

    public async Task<JournalReconciliationTransaction> CreateAsync(JournalReconciliationTransaction journalExpense)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetJournalReconciliationTransactionManager().CreateAsync(journalExpense);
    }

    public async Task<List<JournalReconciliationTransaction>> GetLastRelevantTransactionsAsync(
      int reconciliationTransactionId,
      int organizationId,
      bool loadChildren = false)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
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