using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IGeneralLedgerManager : IGenericRepository<GeneralLedger, int>
  {
    Task<GeneralLedger> GetAsync(int generalLedgerId, int organizationId);
    Task<List<GeneralLedger>> GetLedgerEntriesAsync(int[] ledgerContextIds, int organizationId);
    Task<bool> HasEntriesAsync(int accountId, int organizationId);
  }
}