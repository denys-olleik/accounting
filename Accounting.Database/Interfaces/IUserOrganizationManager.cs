using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IUserOrganizationManager : IGenericRepository<UserOrganization, int>
  {
    Task<UserOrganization> GetAsync(int userId, int organizationId, string databaseName);
    Task<List<Organization>> GetByUserIdAsync(int userId, string? databaseName);
    Task<UserOrganization> CreateAsync(UserOrganization userOrganization, string databaseName);
    Task<List<UserOrganization>> GetAllAsync(int tenantId);
    Task<List<(Organization Organization, Tenant? Tenant)>> GetByEmailAsync(string email, bool searchTenants);
    Task<UserOrganization> GetByEmailAsync(string email, int? selectedOrganizationId, int tenantPublicId);
    Task<List<User>> GetUsersWithOrganizationsAsync(string databaseName);
    Task<UserOrganization> CreateAsync(int userID, int organizationId, string databaseName);
  }
}