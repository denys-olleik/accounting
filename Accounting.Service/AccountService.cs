using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class AccountService
  {
    public async Task<List<Account>> GetAccountBalanceReport(int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetAccountManager().GetAccountBalanceReport(organizationId);
    }

    public async Task<Account> GetAsync(int accountId, int organizationId, string databaseName)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetAccountManager().GetAsync(accountId, organizationId, databaseName);
    }

    public async Task<Account> CreateAsync(Account account)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetAccountManager().CreateAsync(account);
    }

    public async Task<List<Account>> GetAccountOptionsForPaymentReceptionCredit(int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetAccountManager().GetAccountOptionsForPaymentReceptionCredit(organizationId);
    }

    public async Task<List<Account>> GetAccountOptionsForPaymentReceptionDebit(int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetAccountManager().GetAccountOptionsForPaymentReceptionDebit(organizationId);
    }

    public async Task<List<Account>> GetAccountOptionsForInvoiceCreationCredit(int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetAccountManager().GetAccountOptionsForInvoiceCreationCredit(organizationId);
    }

    public async Task<List<Account>> GetAccountOptionsForInvoiceCreationDebit(int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetAccountManager().GetAccountOptionsForInvoiceCreationDebit(organizationId);
    }

    public async Task<int> UpdateAsync(Account account)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetAccountManager().UpdateAsync(account);
    }

    public async Task<Account> GetByAccountNameAsync(string accountName, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetAccountManager().GetByAccountNameAsync(accountName, organizationId);
    }

    public async Task<(List<Account> Accounts, int? NextPageNumber)> GetAllAsync(int page, int pageSize, int organizationId, string databaseName, bool includeJournalEntriesCount, bool includeDescendants)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetAccountManager().GetAllAsync(page, pageSize, organizationId, databaseName, includeJournalEntriesCount, includeDescendants);
    }

    private void PopulateChildrenRecursively(List<Account> children, List<Account> allOrganizationAccountsFlatList)
    {
      foreach (var child in children)
      {
        child.Children = allOrganizationAccountsFlatList.Where(x => x.ParentAccountId == child.AccountID).ToList();

        if (child.Children.Any())
        {
          PopulateChildrenRecursively(child.Children, allOrganizationAccountsFlatList);
        }
      }
    }

    public async Task<List<Account>> GetAllAsync(string accountType, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetAccountManager().GetAllAsync(accountType, organizationId);
    }

    public async Task<bool> ExistsAsync(int accountId, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetAccountManager().ExistsAsync(accountId, organizationId);
    }

    public async Task<List<Account>> GetAllReconciliationExpenseAccountsAsync(int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetAccountManager().GetAllReconciliationExpenseAsync(organizationId);
    }

    public async Task<List<Account>> GetAllReconciliationLiabilitiesAndAssetsAsync(int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetAccountManager().GetAllReconciliationLiabilitiesAndAssetsAsync(organizationId);
    }

    public async Task<string> GetTypeAsync(int accountId)
    {
      FactoryManager factoryManager = new FactoryManager();
      string type = await factoryManager.GetAccountManager().GetTypeAsync(accountId);
      return type;
    }

    public async Task<List<Account>> GetAllAsync(int organizationId, bool includeJournalEntriesCount)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetAccountManager().GetAllAsync(organizationId, includeJournalEntriesCount);
    }

    public async Task<Account> CreateAsync(Account account, string databaseName)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetAccountManager().CreateAsync(account, databaseName);
    }
  }
}