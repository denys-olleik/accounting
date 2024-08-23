using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface ITenantManager : IGenericRepository<Tenant, int>
  {
    Task<bool> ExistsAsync(string email);
  }
}