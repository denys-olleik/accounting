using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IChartOfAccountManager : IGenericRepository<ChartOfAccount, int>
  {
    Task<bool> ExistsAsync(int id, int organizationId);
    Task<List<ChartOfAccount>> GetAccountBalanceReport(int organizationId);
    Task<List<ChartOfAccount>> GetAccountOptionsForInvoiceCreationCredit(int organizationId);
    Task<List<ChartOfAccount>> GetAccountOptionsForInvoiceCreationDebit(int organizationId);
    Task<List<ChartOfAccount>> GetAccountOptionsForPaymentReceptionCredit(int organizationId);
    Task<List<ChartOfAccount>> GetAccountOptionsForPaymentReceptionDebit(int organizationId);
    Task<List<ChartOfAccount>> GetAllAsync(int organizationId);
    Task<List<ChartOfAccount>> GetAllAsync(string accountType, int organizationId);
    Task<List<ChartOfAccount>> GetAllReconciliationExpenseAsync(int organizationId);
    Task<List<ChartOfAccount>> GetAllReconciliationLiabilitiesAndAssetsAsync(int organizationId);
    Task<List<ChartOfAccount>> GetAsync(string[] accountName, int organizationId);
    Task<ChartOfAccount> GetAsync(int id);
    Task<ChartOfAccount> GetAsync(string accountName, int organizationId);
    Task<ChartOfAccount> GetAsync(int id, int organizationId);
    Task<ChartOfAccount> GetByAccountNameAsync(string accountName, int organizationId);
    Task<int> UpdateAsync(ChartOfAccount chartOfAccount);
  }
}