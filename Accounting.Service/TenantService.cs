﻿using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class TenantService
  {
    public async Task<Tenant> CreateAsync(Tenant tenant)
    {
      FactoryManager manager = new FactoryManager();
      return await manager.GetTenantManager().CreateAsync(tenant);
    }

    public async Task<bool> ExistsAsync(string email, int organizationId)
    {
      FactoryManager manager = new FactoryManager();
      return await manager.GetTenantManager().ExistsAsync(email, organizationId);
    }

    public async Task<(List<Tenant> tenants, int? nextPage)> GetAllAsync(
      int page, 
      int pageSize)
    {
      FactoryManager manager = new FactoryManager();
      return await manager.GetTenantManager().GetAllAsync(page, pageSize);
    }

    public async Task UpdateSharedDatabaseName(int tenantID, string? sharedDatabaseName, int organizationId)
    {
      FactoryManager manager = new FactoryManager();
      await manager.GetTenantManager().UpdateSharedDatabaseName(tenantID, sharedDatabaseName, organizationId);
    }

    public async Task UpdateDropletIdAsync(int tenantId, long dropletId, int organizationId)
    {
      FactoryManager manager = new FactoryManager();
      await manager.GetTenantManager().UpdateDropletIdAsync(tenantId, dropletId, organizationId);
    }

    public async Task UpdateSshPrivateAsync(int tenantId, string sshPrivate, int organizationId)
    {
      FactoryManager manager = new FactoryManager();
      await manager.GetTenantManager().UpdateSshPrivateAsync(tenantId, sshPrivate, organizationId);
    }

    public async Task UpdateSshPublicAsync(int tenantId, string sshPublicKey, int organizationId)
    {
      FactoryManager manager = new FactoryManager();
      await manager.GetTenantManager().UpdateSshPublicAsync(tenantId, sshPublicKey, organizationId);
    }
  }
}