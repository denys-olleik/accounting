using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IUserOrganizationManager : IGenericRepository<UserOrganization, int>
  {
    Task<UserOrganization> GetAsync(int userId, int organizationId);
    Task<List<Organization>> GetByUserIdAsync(int userId);
    Task<UserOrganization> CreateAsync(UserOrganization userOrganization, string databaseName);
    Task<List<UserOrganization>> GetAllAsync(int tenantId);
  }
}