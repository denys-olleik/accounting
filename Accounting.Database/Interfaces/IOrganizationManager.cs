using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IOrganizationManager : IGenericRepository<Organization, int>
  {
    Task<Organization> CreateAsync(string organizationName);
    Task<Organization> GetAsync(int organizationId);
    Task<string?> GetPaymentInstructions(int organizationId);
    Task<bool> OrganizationExistsAsync(string name);
    Task<int> UpdateAccountsPayableEmailAsync(int organizationId, string accountsPayableEmail);
    Task<int> UpdateAccountsPayablePhoneAsync(int organizationId, string accountsPayablePhone);
    Task<int> UpdateAccountsReceivableEmailAsync(int organizationId, string accountsReceivableEmail);
    Task<int> UpdateAccountsReceivablePhoneAsync(int organizationId, string accountsReceivablePhone);
    Task<int> UpdateAddressAsync(int organizationId, string address);
    Task<int> UpdateNameAsync(int organizationId, string name);
    Task<int> UpdatePaymentInstructions(int organizationId, string? PaymentInstructions);
    Task<int> UpdateWebsiteAsync(int organizationId, string website);
  }
}