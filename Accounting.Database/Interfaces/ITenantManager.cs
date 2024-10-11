using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface ITenantManager : IGenericRepository<Tenant, int>
  {
    Task<bool> ExistsAsync(string email);
    Task<int> UpdateDropletIdAsync(long dropletId, int organizationId);
    Task<int> UpdateSshPrivateAsync(string sshPrivate, int organizationId);
    Task<int> UpdateSshPublicAsync(string sshPublic, int organizationId);
  }
}