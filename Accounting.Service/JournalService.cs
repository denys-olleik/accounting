using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class JournalService
  {
    private readonly string _databaseName;
    private readonly string _databasePassword;

    public JournalService(string databasePassword = "password", string databaseName = DatabaseThing.DatabaseConstants.Database)
    {
      _databaseName = databaseName;
      _databasePassword = databasePassword;
    }

    public async Task<Journal> CreateAsync(Journal journal)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetJournalManager().CreateAsync(journal);
    }

    public async Task<bool> HasEntriesUpToRootAsync(int accountId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetJournalManager().HasEntriesAsync(accountId, organizationId);
    }

    // Uncomment and use these methods as needed:
    // public async Task<Journal> GetAsync(int journalId, int organizationId)
    // {
    //   var factoryManager = new FactoryManager(_databasePassword, _databaseName);
    //   return await factoryManager.GetJournalManager().GetAsync(journalId, organizationId);
    // }

    // public async Task<List<Journal>> GetLedgerEntriesAsync(int[] ledgerContextIds, int organizationId)
    // {
    //   var factoryManager = new FactoryManager(_databasePassword, _databaseName);
    //   return await factoryManager.GetJournalManager().GetLedgerEntriesAsync(ledgerContextIds, organizationId);
    // }
  }
}