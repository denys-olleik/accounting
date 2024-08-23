using Accounting.Database;

namespace Accounting.Service
{
  public class TenantService
  {
    public async Task<bool> ExistsAsync(string email)
    {
      FactoryManager manager = new FactoryManager();
      return await manager.GetTenantManager().ExistsAsync(email);
    }
  }
}