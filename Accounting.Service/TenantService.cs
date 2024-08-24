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
  }
}