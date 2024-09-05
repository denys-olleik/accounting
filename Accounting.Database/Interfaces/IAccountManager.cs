using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IAccountManager : IGenericRepository<Account, int>
  {
    Task<bool> ExistsAsync(int id, int organizationId);
    Task<List<Account>> GetAccountBalanceReport(int organizationId);
    Task<List<Account>> GetAccountOptionsForInvoiceCreationCredit(int organizationId);
    Task<List<Account>> GetAccountOptionsForInvoiceCreationDebit(int organizationId);
    Task<List<Account>> GetAccountOptionsForPaymentReceptionCredit(int organizationId);
    Task<List<Account>> GetAccountOptionsForPaymentReceptionDebit(int organizationId);
    Task<List<Account>> GetAllAsync(int organizationId);
    Task<List<Account>> GetAllAsync(string accountType, int organizationId);
    Task<List<Account>> GetAllReconciliationExpenseAsync(int organizationId);
    Task<List<Account>> GetAllReconciliationLiabilitiesAndAssetsAsync(int organizationId);
    Task<List<Account>> GetAsync(string[] accountName, int organizationId);
    Task<Account> GetAsync(int id);
    Task<Account> GetAsync(string accountName, int organizationId);
    Task<Account> GetAsync(int id, int organizationId);
    Task<Account> GetByAccountNameAsync(string accountName, int organizationId);
    Task<int> UpdateAsync(Account account);
  }
}