using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class OrganizationService : BaseService
  {
    public OrganizationService() : base()
    {

    }

    public OrganizationService(string databaseName, string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<Organization> CreateAsync(string organizationName)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetOrganizationManager().CreateAsync(organizationName);
    }

    public async Task<Organization> CreateAsync(string organizationName, string databaseName)
    {
      var factoryManager = new FactoryManager(_databasePassword, databaseName);
      return await factoryManager.GetOrganizationManager().CreateAsync(organizationName, databaseName);
    }

    public async Task InsertSampleOrganizationDataAsync(string sampleSqlDataToInsert)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      await factoryManager.GetOrganizationManager().InsertSampleOrganizationDataAsync(sampleSqlDataToInsert);
    }

    public async Task<int> DeleteAsync(int organizationId, string databaseName)
    {
      var factoryManager = new FactoryManager(_databasePassword, databaseName);
      return await factoryManager.GetOrganizationManager().DeleteAsync(organizationId, databaseName);
    }

    public async Task<List<Organization>> GetAllAsync(string databaseName)
    {
      var factoryManager = new FactoryManager(_databasePassword, databaseName);
      return await factoryManager.GetOrganizationManager().GetAllAsync(databaseName);
    }

    public async Task<Organization> GetAsync(int organizationId, string databaseName)
    {
      var factoryManager = new FactoryManager(_databasePassword, databaseName);
      return await factoryManager.GetOrganizationManager().GetAsync(organizationId, databaseName);
    }

    public async Task<Organization> GetAsync(string name, bool searchTenants)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetOrganizationManager().GetAsync(name, searchTenants);
    }

    public async Task<Organization> GetAsync(string name, string databaseName)
    {
      var factoryManager = new FactoryManager(_databasePassword, databaseName);
      return await factoryManager.GetOrganizationManager().GetAsync(name, databaseName);
    }

    public async Task<string?> GetPaymentInstructions(int organizationId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetOrganizationManager().GetPaymentInstructions(organizationId);
    }

    public async Task UpdateAccountsPayableEmailAsync(int organizationId, string accountsPayableEmail)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      await factoryManager.GetOrganizationManager().UpdateAccountsPayableEmailAsync(organizationId, accountsPayableEmail);
    }

    public async Task UpdateAccountsPayablePhoneAsync(int organizationId, string accountsPayablePhone)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      await factoryManager.GetOrganizationManager().UpdateAccountsPayablePhoneAsync(organizationId, accountsPayablePhone);
    }

    public async Task UpdateAccountsReceivableEmailAsync(int organizationId, string accountsReceivableEmail)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      await factoryManager.GetOrganizationManager().UpdateAccountsReceivableEmailAsync(organizationId, accountsReceivableEmail);
    }

    public async Task UpdateAccountsReceivablePhoneAsync(int organizationId, string accountsReceivablePhone)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      await factoryManager.GetOrganizationManager().UpdateAccountsReceivablePhoneAsync(organizationId, accountsReceivablePhone);
    }

    public async Task<int> UpdateAddressAsync(int organizationId, string address)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetOrganizationManager().UpdateAddressAsync(organizationId, address);
    }

    public async Task UpdateAsync(int organizationId, string name, string databaseName)
    {
      var factoryManager = new FactoryManager(_databasePassword, databaseName);
      await factoryManager.GetOrganizationManager().UpdateAsync(organizationId, name, databaseName);
    }

    public async Task<int> UpdateNameAsync(int organizationId, string name)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetOrganizationManager().UpdateNameAsync(organizationId, name);
    }

    public async Task<int> UpdatePaymentInstructions(int organizationId, string? paymentInstructions)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetOrganizationManager().UpdatePaymentInstructions(organizationId, paymentInstructions);
    }

    public async Task<int> UpdateWebsiteAsync(int organizationId, string website)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetOrganizationManager().UpdateWebsiteAsync(organizationId, website);
    }
  }
}