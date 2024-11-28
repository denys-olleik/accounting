using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class SecretService
  {
    private readonly string _databaseName;

    public SecretService(string databaseName = "Accounting")
    {
      _databaseName = databaseName;
    }

    public async Task<Secret> CreateAsync(
      bool master,
      string? value,
      string? type,
      string? purpose,
      int organizationId,
      int createdById)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetSecretManager(_databaseName)
        .CreateAsync(master, value, type, purpose, organizationId, createdById);
    }

    public async Task<int> DeleteAsync(int id, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetSecretManager(_databaseName)
        .DeleteAsync(id, organizationId);
    }

    public async Task<int> DeleteMasterAsync(int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetSecretManager(_databaseName)
        .DeleteMasterAsync(organizationId);
    }

    public async Task<List<Secret>> GetAllAsync(int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetSecretManager(_databaseName)
        .GetAllAsync(organizationId);
    }

    public async Task<Secret> GetAsync(int id, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetSecretManager(_databaseName)
        .GetAsync(id, organizationId);
    }

    public async Task<Secret?> GetAsync(string type, int? organizationId = null)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetSecretManager(_databaseName)
        .GetAsync(type, organizationId);
    }

    public async Task<Secret?> GetMasterAsync(int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetSecretManager(_databaseName)
        .GetMasterAsync(organizationId);
    }
  }
}
