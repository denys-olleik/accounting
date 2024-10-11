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

    public async Task UpdateDropletIdAsync(long dropletId, int organizationId)
    {
      FactoryManager manager = new FactoryManager();
      await manager.GetTenantManager().UpdateDropletIdAsync(dropletId, organizationId);
    }

    public async Task UpdateSshPrivateAsync(string sshPrivate, int organizationId)
    {
      FactoryManager manager = new FactoryManager();
      await manager.GetTenantManager().UpdateSshPrivateAsync(sshPrivate, organizationId);
    }

    public async Task UpdateSshPublicAsync(string SshPublic, int organizationId)
    {
      FactoryManager manager = new FactoryManager();
      await manager.GetTenantManager().UpdateSshPublicAsync(SshPublic, organizationId);
    }
  }
}