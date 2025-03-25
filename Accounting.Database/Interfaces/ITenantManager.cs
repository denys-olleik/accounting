using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface ITenantManager : IGenericRepository<Tenant, int>
  {
    Task<Tenant> GetAsync(string tenantPublicId);

    Task<bool> ExistsAsync(string email);
    Task<(List<Tenant> tenants, int? nextPage)> GetAllAsync(
      int page,
      int pageSize);
    Task<int> UpdateDatabaseName(
      int tenantID,
      string? databaseName);
    Task<int> UpdateDropletIdAsync(
      int tenantId,
      long dropletId);
    Task<int> UpdateSshPrivateAsync(
      int tenantId,
      string sshPrivate);
    Task<int> UpdateSshPublicAsync(
      int tenantId,
      string sshPublic);
    Task<Tenant> GetAsync(int tenantId);
    Task<int> DeleteAsync(int tenantID);
    Task<int> UpdateEmailAsync(int tenantId, string email);
    Task<int> UpdateUserAsync(string email, string firstName, string lastName);
    Task<int> UpdateIpv4Async(int tenantId, string ipAddress);
    Task<bool> TenantExistsAsync(string? databaseName);
    Task<Tenant> GetByEmailAsync(string? email);
    Task<Tenant> GetByDatabaseNameAsync(string databaseName);
  }
}