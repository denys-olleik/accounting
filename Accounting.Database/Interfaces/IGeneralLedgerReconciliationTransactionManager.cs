using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IGeneralLedgerReconciliationTransactionManager : IGenericRepository<GeneralLedgerReconciliationTransaction, int>
  {
    Task<List<GeneralLedgerReconciliationTransaction>> GetLastTransactionAsync(int reconciliationTransactionId, int organizationId, bool loadChildren = false);
  }
}