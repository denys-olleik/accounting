using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class JournalService
  {
    public async Task<Journal> CreateAsync(Journal journal)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetJournalManager().CreateAsync(journal);
    }

    //public async Task<Journal> GetAsync(int journalId, int organizationId)
    //{
    //  FactoryManager factoryManager = new FactoryManager();
    //  return await factoryManager.GetJournalManager().GetAsync(journalId, organizationId);
    //}

    //public async Task<List<Journal>> GetLedgerEntriesAsync(int[] ledgerContextIds, int organizationId)
    //{
    //  FactoryManager factoryManager = new FactoryManager();
    //  return await factoryManager.GetJournalManager().GetLedgerEntriesAsync(ledgerContextIds, organizationId);
    //}

    public async Task<bool> HasEntriesUpToRootAsync(int accountId, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetJournalManager().HasEntriesAsync(accountId, organizationId);
    }
  }
}