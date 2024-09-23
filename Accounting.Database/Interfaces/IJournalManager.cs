using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IJournalManager : IGenericRepository<Journal, int>
  {
    Task<Journal> GetAsync(int journalId, int organizationId);
    Task<List<Journal>> GetLedgerEntriesAsync(int[] ledgerContextIds, int organizationId);
    Task<bool> HasEntriesAsync(int accountId, int organizationId);
  }
}