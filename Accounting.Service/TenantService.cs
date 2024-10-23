using Accounting.Business;
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

    public async Task<bool> ExistsAsync(string email)
    {
      FactoryManager manager = new FactoryManager();
      return await manager.GetTenantManager().ExistsAsync(email);
    }

    public async Task<(List<Tenant> tenants, int? nextPage)> GetAllAsync(
      int page, 
      int pageSize)
    {
      FactoryManager manager = new FactoryManager();
      return await manager.GetTenantManager().GetAllAsync(page, pageSize);
    }

    public async Task UpdateSharedDatabaseName(int tenantID, string? sharedDatabaseName)
    {
      FactoryManager manager = new FactoryManager();
      await manager.GetTenantManager().UpdateSharedDatabaseName(tenantID, sharedDatabaseName);
    }

    public async Task UpdateDropletIdAsync(int tenantId, long dropletId)
    {
      FactoryManager manager = new FactoryManager();
      await manager.GetTenantManager().UpdateDropletIdAsync(tenantId, dropletId);
    }

    public async Task UpdateSshPrivateAsync(int tenantId, string sshPrivate)
    {
      FactoryManager manager = new FactoryManager();
      await manager.GetTenantManager().UpdateSshPrivateAsync(tenantId, sshPrivate);
    }

    public async Task UpdateSshPublicAsync(int tenantId, string sshPublicKey)
    {
      FactoryManager manager = new FactoryManager();
      await manager.GetTenantManager().UpdateSshPublicAsync(tenantId, sshPublicKey);
    }

    public async Task<Tenant> GetAsync(int tenantId)
    {
      FactoryManager manager = new FactoryManager();
      return await manager.GetTenantManager().GetAsync(tenantId);
    }
  }
}