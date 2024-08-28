using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class SecretService
  {
    public async Task<Secret> CreateAsync(
      string? key, 
      string? value, 
      int organizationId, 
      int createdById)
    {
      FactoryManager manager = new FactoryManager();
      return await manager
        .GetSecretManager()
        .CreateAsync(key, value, organizationId, createdById);
    }

    public async Task<List<Secret>> GetAllAsync(int organizationId)
    {
      FactoryManager manager = new FactoryManager();
      return await manager.GetSecretManager().GetAllAsync(organizationId);
    }
  }
}