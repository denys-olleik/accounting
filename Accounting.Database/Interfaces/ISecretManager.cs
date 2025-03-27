using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface ISecretManager : IGenericRepository<Secret, int>
  {
    Task<Secret> CreateAsync(bool master, string? value, string? vendor, string? purpose, int organizationId, int createdById, int tenantId);
    Task<int> DeleteAsync(int id);
    Task<int> DeleteMasterAsync(int tenantId);
    Task<List<Secret>> GetAllAsync(int organizationId);
    Task<Secret> GetAsync(int id, int organizationId);
    Task<Secret?> GetAsync(string key, int tenant, int? organizationId);
    Task<Secret?> GetAsync(string type);
    Task<Secret?> GetMasterAsync(int tenantId);
  }
}