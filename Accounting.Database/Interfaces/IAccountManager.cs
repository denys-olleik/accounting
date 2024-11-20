using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IAccountManager : IGenericRepository<Account, int>
  {
    Task<Account> CreateAsync(Account account, string databaseName);
    Task<bool> ExistsAsync(int id, int organizationId);
    Task<List<Account>> GetAccountBalanceReport(int organizationId);
    Task<List<Account>> GetAccountOptionsForInvoiceCreationCredit(int organizationId);
    Task<List<Account>> GetAccountOptionsForInvoiceCreationDebit(int organizationId);
    Task<List<Account>> GetAccountOptionsForPaymentReceptionCredit(int organizationId);
    Task<List<Account>> GetAccountOptionsForPaymentReceptionDebit(int organizationId);
    Task<List<Account>> GetAllAsync(string accountType, int organizationId);
    Task<(List<Account> accounts, int? nextPage)> GetAllAsync(
      int page, 
      int pageSize, 
      int organizationId, 
      string databaseName, 
      bool includeJournalEntriesCount, 
      bool includeDescendants);
    Task<List<Account>> GetAllAsync(int organizationId, bool includeJournalEntriesCount);
    Task<List<Account>> GetAllReconciliationExpenseAsync(int organizationId);
    Task<List<Account>> GetAllReconciliationLiabilitiesAndAssetsAsync(int organizationId);
    Task<List<Account>> GetAsync(string[] accountName, int organizationId);
    Task<Account> GetAsync(int id);
    Task<Account> GetAsync(string accountName, int organizationId);
    Task<Account> GetAsync(int id, int organizationId, string databaseName);
    Task<Account> GetByAccountNameAsync(string accountName, int organizationId);
    Task<string> GetTypeAsync(int accountId);
    Task<int> UpdateAsync(Account account);
  }
}