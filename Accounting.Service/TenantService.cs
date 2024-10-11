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