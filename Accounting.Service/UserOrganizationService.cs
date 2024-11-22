using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class UserOrganizationService
  {
    public async Task<List<Organization>> GetByUserIdAsync(int userId, string databaseName)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetUserOrganizationManager().GetByUserIdAsync(userId, databaseName);
    }

    public async Task<UserOrganization> GetAsync(int userId, int organizationId, string databaseName)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetUserOrganizationManager().GetAsync(userId, organizationId, databaseName);
    }

    public async Task<UserOrganization> CreateAsync(UserOrganization userOrganization)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetUserOrganizationManager().CreateAsync(userOrganization);
    }

    public async Task CreateAsync(UserOrganization userOrganization, string databaseName)
    {
      FactoryManager factoryManager = new FactoryManager();
      await factoryManager.GetUserOrganizationManager().CreateAsync(userOrganization, databaseName);
    }

    public async Task<List<UserOrganization>> GetAllAsync(int tenantId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetUserOrganizationManager().GetAllAsync(tenantId);
    }

    public async Task<List<(Organization Organization, Tenant? Tenant)>> GetByEmailAsync(string email, bool searchTenants)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetUserOrganizationManager().GetByEmailAsync(email, searchTenants);
    }

    public async Task<UserOrganization> GetByEmailAsync(string email, int? selectedOrganizationId, int tenantPublicId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetUserOrganizationManager().GetByEmailAsync(email, selectedOrganizationId, tenantPublicId);
    }

    public async Task<List<User>> GetUsersWithOrganizationsAsync(string databaseName)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetUserOrganizationManager().GetUsersWithOrganizationsAsync(databaseName);
    }

    public async Task<UserOrganization> CreateAsync(int userID, int organizationId, string databaseName)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetUserOrganizationManager().CreateAsync(userID, organizationId, databaseName);
    }

    public async Task UpdateUserOrganizationsAsync(int userID, List<int> selectedOrganizationIds, string databaseName)
    {
      FactoryManager factoryManager = new FactoryManager();
      await factoryManager.GetUserOrganizationManager().UpdateUserOrganizationsAsync(userID, selectedOrganizationIds, databaseName);
    }
  }
}