using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IReconciliationExpenseCategoryManager : IGenericRepository<ReconciliationExpenseCategory, int>
  {
    Task<List<ReconciliationExpenseCategory>> GetAllAsync(int organizationId);
    Task<ReconciliationExpenseCategory> GetAsync(int reconciliationExpenseCategoryId);
  }
}