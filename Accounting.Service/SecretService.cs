using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class SecretService : BaseService
  {
    public SecretService() : base()
    {

    }

    public SecretService(
      string databaseName, 
      string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<Secret> CreateAsync(
        bool master,
        string? value,
        string? type,
        string? purpose,
        int organizationId,
        int createdById,
        int tenantId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetSecretManager()
          .CreateAsync(master, value, type, purpose, organizationId, createdById, tenantId);
    }

    public async Task<int> DeleteAsync(int id, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetSecretManager()
          .DeleteAsync(id, organizationId);
    }

    public async Task<int> DeleteMasterAsync(int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetSecretManager()
          .DeleteMasterAsync(organizationId);
    }

    public async Task<List<Secret>> GetAllAsync(int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetSecretManager()
          .GetAllAsync(organizationId);
    }

    public async Task<Secret> GetAsync(int id, int tenantId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetSecretManager()
          .GetAsync(id, tenantId);
    }

    public async Task<Secret> GetAsync(string type, int tenantId, int? organizationId = null)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetSecretManager()
          .GetAsync(type, tenantId, organizationId);
    }

    public async Task<Secret?> GetMasterAsync(int tenantId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetSecretManager()
          .GetMasterAsync(tenantId);
    }
  }
}
