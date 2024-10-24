﻿using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class OrganizationService
  {
    public async Task<Organization> CreateAsync(string? organizationName)
    {
      throw new NotImplementedException();
    }

    public async Task<Organization> GetAsync(int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetOrganizationManager().GetAsync(organizationId);
    }

    public async Task<string?> GetPaymentInstructions(int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetOrganizationManager().GetPaymentInstructions(organizationId);
    }

    public async Task<bool> OrganizationExistsAsync(string name)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetOrganizationManager().OrganizationExistsAsync(name);
    }

    public async Task UpdateAccountsPayableEmailAsync(int organizationId, string accountsPayableEmail)
    {
      FactoryManager factoryManager = new FactoryManager();
      await factoryManager.GetOrganizationManager().UpdateAccountsPayableEmailAsync(organizationId, accountsPayableEmail);
    }

    public async Task UpdateAccountsPayablePhoneAsync(int organizationId, string accountsPayablePhone)
    {
      FactoryManager factoryManager = new FactoryManager();
      await factoryManager.GetOrganizationManager().UpdateAccountsPayablePhoneAsync(organizationId, accountsPayablePhone);
    }

    public async Task UpdateAccountsReceivableEmailAsync(int organizationId, string accountsReceivableEmail)
    {
      FactoryManager factoryManager = new FactoryManager();
      await factoryManager.GetOrganizationManager().UpdateAccountsReceivableEmailAsync(organizationId, accountsReceivableEmail);
    }

    public async Task UpdateAccountsReceivablePhoneAsync(int organizationId, string accountsReceivablePhone)
    {
      FactoryManager factoryManager = new FactoryManager();
      await factoryManager.GetOrganizationManager().UpdateAccountsReceivablePhoneAsync(organizationId, accountsReceivablePhone);
    }

    public async Task<int> UpdateAddressAsync(int organizationId, string address)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetOrganizationManager().UpdateAddressAsync(organizationId, address);
    }

    public async Task<int> UpdateNameAsync(int organizationId, string name)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetOrganizationManager().UpdateNameAsync(organizationId, name);
    }

    public async Task<int> UpdatePaymentInstructions(int organizationId, string? paymentInstructions)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetOrganizationManager().UpdatePaymentInstructions(organizationId, paymentInstructions);
    }

    public async Task<int> UpdateWebsiteAsync(int organizationId, string website)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetOrganizationManager().UpdateWebsiteAsync(organizationId, website);
    }
  }
}