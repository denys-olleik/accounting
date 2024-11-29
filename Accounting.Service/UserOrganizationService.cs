using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class UserOrganizationService
  {
    private readonly string _databaseName;

    public UserOrganizationService(string databaseName = DatabaseThing.DatabaseConstants.Database)
    {
      _databaseName = databaseName;
    }

    public async Task<List<Organization>> GetByUserIdAsync(int userId, string databaseName)
    {
      var factoryManager = new FactoryManager(databaseName);
      return await factoryManager.GetUserOrganizationManager().GetByUserIdAsync(userId, databaseName);
    }

    public async Task<UserOrganization> GetAsync(int userId, int organizationId, string databaseName)
    {
      var factoryManager = new FactoryManager(databaseName);
      return await factoryManager.GetUserOrganizationManager().GetAsync(userId, organizationId, databaseName);
    }

    public async Task<UserOrganization> CreateAsync(UserOrganization userOrganization)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetUserOrganizationManager().CreateAsync(userOrganization);
    }

    public async Task CreateAsync(UserOrganization userOrganization, string databaseName)
    {
      var factoryManager = new FactoryManager(databaseName);
      await factoryManager.GetUserOrganizationManager().CreateAsync(userOrganization, databaseName);
    }

    public async Task<List<UserOrganization>> GetAllAsync(int tenantId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetUserOrganizationManager().GetAllAsync(tenantId);
    }

    public async Task<List<(Organization Organization, Tenant? Tenant)>> GetByEmailAsync(string email, bool searchTenants)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetUserOrganizationManager().GetByEmailAsync(email, searchTenants);
    }

    public async Task<UserOrganization> GetByEmailAsync(string email, int? selectedOrganizationId, int tenantPublicId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetUserOrganizationManager().GetByEmailAsync(email, selectedOrganizationId, tenantPublicId);
    }

    public async Task<List<User>> GetUsersWithOrganizationsAsync(string databaseName)
    {
      var factoryManager = new FactoryManager(databaseName);
      return await factoryManager.GetUserOrganizationManager().GetUsersWithOrganizationsAsync(databaseName);
    }

    public async Task<UserOrganization> CreateAsync(int userID, int organizationId, string databaseName)
    {
      var factoryManager = new FactoryManager(databaseName);
      return await factoryManager.GetUserOrganizationManager().CreateAsync(userID, organizationId, databaseName);
    }

    public async Task UpdateUserOrganizationsAsync(int userID, List<int> selectedOrganizationIds, string databaseName)
    {
      var factoryManager = new FactoryManager(databaseName);
      await factoryManager.GetUserOrganizationManager().UpdateUserOrganizationsAsync(userID, selectedOrganizationIds, databaseName);
    }

    public async Task<int> DeleteByOrganizationIdAsync(int organizationId, string databaseName)
    {
      var factoryManager = new FactoryManager(databaseName);
      return await factoryManager.GetUserOrganizationManager().DeleteByOrganizationIdAsync(organizationId, databaseName);
    }
  }
}