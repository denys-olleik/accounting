using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface ITenantManager : IGenericRepository<Tenant, int>
  {
    Task<bool> ExistsAsync(string email, int organizationId);
    Task<(List<Tenant> tenants, int? nextPage)> GetAllAsync(
      int page, 
      int pageSize, 
      int organizationId);
    Task<int> UpdateSharedDatabaseName(
      int tenantID, 
      string? sharedDatabaseName, 
      int organizationId);
    Task<int> UpdateDropletIdAsync(
      int tenantId, 
      long dropletId, 
      int organizationId);
    Task<int> UpdateSshPrivateAsync(
      int tenantId, 
      string sshPrivate, 
      int organizationId);
    Task<int> UpdateSshPublicAsync(
      int tenantId, 
      string sshPublic, 
      int organizationId);
  }
}