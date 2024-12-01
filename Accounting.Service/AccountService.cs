using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class AccountService
  {
    private readonly string _databaseName;

    public AccountService(RequestContext context)
    {
      _databaseName = context.DatabaseName;
    }

    public async Task<List<Account>> GetAccountBalanceReport(int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetAccountManager().GetAccountBalanceReport(organizationId);
    }

    public async Task<Account> GetAsync(int accountId, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetAccountManager().GetAsync(accountId, organizationId);
    }

    public async Task<Account> CreateAsync(Account account)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetAccountManager().CreateAsync(account);
    }

    public async Task<List<Account>> GetAccountOptionsForPaymentReceptionCredit(int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetAccountManager().GetAccountOptionsForPaymentReceptionCredit(organizationId);
    }

    public async Task<List<Account>> GetAccountOptionsForPaymentReceptionDebit(int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetAccountManager().GetAccountOptionsForPaymentReceptionDebit(organizationId);
    }

    public async Task<List<Account>> GetAccountOptionsForInvoiceCreationCredit(int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetAccountManager().GetAccountOptionsForInvoiceCreationCredit(organizationId);
    }

    public async Task<List<Account>> GetAccountOptionsForInvoiceCreationDebit(int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetAccountManager().GetAccountOptionsForInvoiceCreationDebit(organizationId);
    }

    public async Task<int> UpdateAsync(Account account)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetAccountManager().UpdateAsync(account);
    }

    public async Task<Account> GetByAccountNameAsync(string accountName, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetAccountManager().GetByAccountNameAsync(accountName, organizationId);
    }

    public async Task<(List<Account> Accounts, int? NextPageNumber)> GetAllAsync(
        int page,
        int pageSize,
        int organizationId,
        bool includeJournalEntriesCount,
        bool includeDescendants)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetAccountManager().GetAllAsync(page, pageSize, organizationId, includeJournalEntriesCount, includeDescendants);
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
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetAccountManager().GetAllAsync(accountType, organizationId);
    }

    public async Task<bool> ExistsAsync(int accountId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetAccountManager().ExistsAsync(accountId, organizationId);
    }

    public async Task<List<Account>> GetAllReconciliationExpenseAccountsAsync(int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetAccountManager().GetAllReconciliationExpenseAsync(organizationId);
    }

    public async Task<List<Account>> GetAllReconciliationLiabilitiesAndAssetsAsync(int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetAccountManager().GetAllReconciliationLiabilitiesAndAssetsAsync(organizationId);
    }

    public async Task<string> GetTypeAsync(int accountId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetAccountManager().GetTypeAsync(accountId);
    }

    public async Task<List<Account>> GetAllAsync(int organizationId, bool includeJournalEntriesCount)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetAccountManager().GetAllAsync(organizationId, includeJournalEntriesCount);
    }
  }
}
