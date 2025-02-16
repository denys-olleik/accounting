using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class TenantService : BaseService
  {
    public TenantService() : base()
    {

    }

    public TenantService(
      string databaseName,
      string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<int> UpdateUserAsync(string email, string firstName, string lastName)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetTenantManager().UpdateUserAsync(email, firstName, lastName);
    }

    public async Task<Tenant> CreateAsync(Tenant tenant)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetTenantManager().CreateAsync(tenant);
    }

    public async Task<int> DeleteAsync(int tenantID)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetTenantManager().DeleteAsync(tenantID);
    }

    public async Task<bool> ExistsAsync(string email)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetTenantManager().ExistsAsync(email);
    }

    public async Task<(List<Tenant> tenants, int? nextPage)> GetAllAsync(int page, int pageSize)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetTenantManager().GetAllAsync(page, pageSize);
    }

    public async Task<Tenant> GetAsync(int tenantId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetTenantManager().GetAsync(tenantId);
    }

    public async Task<int> UpdateEmailAsync(int tenantId, string email)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetTenantManager().UpdateEmailAsync(tenantId, email);
    }

    public async Task UpdateDatabaseName(int tenantID, string? databaseName)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      await factoryManager.GetTenantManager().UpdateDatabaseName(tenantID, databaseName);
    }

    public async Task UpdateDropletIdAsync(int tenantId, long dropletId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      await factoryManager.GetTenantManager().UpdateDropletIdAsync(tenantId, dropletId);
    }

    public async Task UpdateSshPrivateAsync(int tenantId, string sshPrivate)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      await factoryManager.GetTenantManager().UpdateSshPrivateAsync(tenantId, sshPrivate);
    }

    public async Task UpdateSshPublicAsync(int tenantId, string sshPublicKey)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      await factoryManager.GetTenantManager().UpdateSshPublicAsync(tenantId, sshPublicKey);
    }

    public async Task<int> UpdateIpv4Async(int tenantId, string ipAddress)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetTenantManager().UpdateIpv4Async(tenantId, ipAddress);
    }

    public async Task<bool> TenantExistsAsync(string? databaseName)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetTenantManager().TenantExistsAsync(databaseName);
    }
  }
}