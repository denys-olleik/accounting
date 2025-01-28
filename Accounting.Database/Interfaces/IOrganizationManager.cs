using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IOrganizationManager : IGenericRepository<Organization, int>
  {
    Task<Organization> CreateAsync(string organizationName);
    Task<Organization> CreateAsync(string organizationName, string databaseName);
    Task InsertSampleOrganizationDataAsync(string sampleSqlDataToInsert);
    Task<int> DeleteAsync(int organizationId, string databaseName);
    Task<List<Organization>> GetAllAsync();
    Task<Organization> GetAsync(int organizationId, string databaseName);
    Task<Organization> GetAsync(string name, bool searchTenants);
    Task<Organization> GetAsync(string name);
    Task<string?> GetPaymentInstructions(int organizationId);
    Task<int> UpdateAccountsPayableEmailAsync(int organizationId, string accountsPayableEmail);
    Task<int> UpdateAccountsPayablePhoneAsync(int organizationId, string accountsPayablePhone);
    Task<int> UpdateAccountsReceivableEmailAsync(int organizationId, string accountsReceivableEmail);
    Task<int> UpdateAccountsReceivablePhoneAsync(int organizationId, string accountsReceivablePhone);
    Task<int> UpdateAddressAsync(int organizationId, string address);
    Task<int> UpdateAsync(int organizationId, string name, string databaseName);
    Task<int> UpdateNameAsync(int organizationId, string name);
    Task<int> UpdatePaymentInstructions(int organizationId, string? PaymentInstructions);
    Task<int> UpdateWebsiteAsync(int organizationId, string website);
  }
}