using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class ChartOfAccountService
  {
    public async Task<List<ChartOfAccount>> GetAllAsync(int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetChartOfAccountManager().GetAllAsync(organizationId);
    }

    public async Task<List<ChartOfAccount>> GetAccountBalanceReport(int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetChartOfAccountManager().GetAccountBalanceReport(organizationId);
    }

    public async Task<ChartOfAccount> GetAsync(int chartOfAccountId, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetChartOfAccountManager().GetAsync(chartOfAccountId, organizationId);
    }

    public async Task<ChartOfAccount> CreateAsync(ChartOfAccount account)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetChartOfAccountManager().CreateAsync(account);
    }

    public async Task<List<ChartOfAccount>> GetAccountOptionsForPaymentReceptionCredit(int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetChartOfAccountManager().GetAccountOptionsForPaymentReceptionCredit(organizationId);
    }

    public async Task<List<ChartOfAccount>> GetAccountOptionsForPaymentReceptionDebit(int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetChartOfAccountManager().GetAccountOptionsForPaymentReceptionDebit(organizationId);
    }

    public async Task<List<ChartOfAccount>> GetAccountOptionsForInvoiceCreationCredit(int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetChartOfAccountManager().GetAccountOptionsForInvoiceCreationCredit(organizationId);
    }

    public async Task<List<ChartOfAccount>> GetAccountOptionsForInvoiceCreationDebit(int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetChartOfAccountManager().GetAccountOptionsForInvoiceCreationDebit(organizationId);
    }

    public async Task<int> UpdateAsync(ChartOfAccount chartOfAccount)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetChartOfAccountManager().UpdateAsync(chartOfAccount);
    }

    public async Task<ChartOfAccount> GetByAccountNameAsync(string accountName, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetChartOfAccountManager().GetByAccountNameAsync(accountName, organizationId);
    }

    public async Task<List<ChartOfAccount>> GetAllHierachicalAsync(int organizationId)
    {
      var allOrganizationAccountsFlatList = await GetAllAsync(organizationId);
      var rootAccounts = allOrganizationAccountsFlatList.Where(x => x.ParentChartOfAccountId == null).ToList();

      foreach (var account in rootAccounts)
      {
        account.Children = allOrganizationAccountsFlatList.Where(x => x.ParentChartOfAccountId == account.ChartOfAccountID).ToList();

        if (account.Children.Any())
        {
          PopulateChildrenRecursively(account.Children, allOrganizationAccountsFlatList);
        }
      }

      return rootAccounts;
    }

    private void PopulateChildrenRecursively(List<ChartOfAccount> children, List<ChartOfAccount> allOrganizationAccountsFlatList)
    {
      foreach (var child in children)
      {
        child.Children = allOrganizationAccountsFlatList.Where(x => x.ParentChartOfAccountId == child.ChartOfAccountID).ToList();

        if (child.Children.Any())
        {
          PopulateChildrenRecursively(child.Children, allOrganizationAccountsFlatList);
        }
      }
    }

    public async Task<List<ChartOfAccount>> GetAllAsync(string accountType, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetChartOfAccountManager().GetAllAsync(accountType, organizationId);
    }

    public async Task<bool> ExistsAsync(int chartOfAccountId, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetChartOfAccountManager().ExistsAsync(chartOfAccountId, organizationId);
    }

    public async Task<List<ChartOfAccount>> GetAllReconciliationExpenseAsync(int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetChartOfAccountManager().GetAllReconciliationExpenseAsync(organizationId);
    }

    public async Task<List<ChartOfAccount>> GetAllReconciliationLiabilitiesAndAssetsAsync(int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetChartOfAccountManager().GetAllReconciliationLiabilitiesAndAssetsAsync(organizationId);
    }
  }
}