using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class SecretService : BaseService
  {
    public SecretService() : base()
    {

    }

    public SecretService(string databaseName, string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<Secret> CreateAsync(
      bool master,
      string? value,
      string? type,
      string? purpose,
      int organizationId,
      int createdById)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetSecretManager()
        .CreateAsync(master, value, type, purpose, organizationId, createdById);
    }

    public async Task<int> DeleteAsync(int id, int organizationId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetSecretManager()
        .DeleteAsync(id, organizationId);
    }

    public async Task<int> DeleteMasterAsync(int organizationId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetSecretManager()
        .DeleteMasterAsync(organizationId);
    }

    public async Task<List<Secret>> GetAllAsync(int organizationId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetSecretManager()
        .GetAllAsync(organizationId);
    }

    public async Task<Secret> GetAsync(int id, int organizationId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetSecretManager()
        .GetAsync(id, organizationId);
    }

    public async Task<Secret?> GetAsync(string? type, int organizationId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetSecretManager()
        .GetAsync(type, organizationId);
    }

    public async Task<Secret> GetAsync(string type)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetSecretManager()
        .GetAsync(type);
    }

    public async Task<Secret?> GetMasterAsync(int organizationId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetSecretManager()
        .GetMasterAsync(organizationId);
    }
  }
}
