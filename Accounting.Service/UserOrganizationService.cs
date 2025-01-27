using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class UserOrganizationService : BaseService
  {
    public UserOrganizationService() : base()
    {

    }

    public UserOrganizationService(string databaseName, string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<List<Organization>> GetByUserIdAsync(int userId, string databasePassword, string databaseName)
    {
      var factoryManager = new FactoryManager(databasePassword, databaseName);
      return await factoryManager.GetUserOrganizationManager().GetByUserIdAsync(userId, databaseName);
    }

    public async Task<UserOrganization> GetAsync(int userId, int organizationId, string databaseName = DatabaseThing.DatabaseConstants.Database, string databasePassword = "password")
    {
      var factoryManager = new FactoryManager(databasePassword, databaseName);
      return await factoryManager.GetUserOrganizationManager().GetAsync(userId, organizationId, databaseName, databasePassword);
    }

    public async Task<UserOrganization> CreateAsync(UserOrganization userOrganization)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetUserOrganizationManager().CreateAsync(userOrganization);
    }

    public async Task CreateAsync(UserOrganization userOrganization, string databasePassword, string databaseName)
    {
      var factoryManager = new FactoryManager(databasePassword, databaseName);
      await factoryManager.GetUserOrganizationManager().CreateAsync(userOrganization, databaseName);
    }

    public async Task<List<UserOrganization>> GetAllAsync(int tenantId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetUserOrganizationManager().GetAllAsync(tenantId);
    }

    public async Task<List<(Organization Organization, Tenant? Tenant)>> GetByEmailAsync(string email, bool searchTenants)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetUserOrganizationManager().GetByEmailAsync(email, searchTenants);
    }

    public async Task<UserOrganization> GetByEmailAsync(string email, int? selectedOrganizationId, int tenantPublicId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetUserOrganizationManager().GetByEmailAsync(email, selectedOrganizationId, tenantPublicId);
    }

    public async Task<List<User>> GetUsersWithOrganizationsAsync(string databasePassword, string databaseName)
    {
      var factoryManager = new FactoryManager(databasePassword, databaseName);
      return await factoryManager.GetUserOrganizationManager().GetUsersWithOrganizationsAsync(databaseName);
    }

    public async Task<UserOrganization> CreateAsync(int userID, int organizationId, string databasePassword = "password", string databaseName = DatabaseThing.DatabaseConstants.Database)
    {
      var factoryManager = new FactoryManager(databasePassword, databaseName);
      return await factoryManager.GetUserOrganizationManager().CreateAsync(userID, organizationId, databaseName);
    }

    public async Task UpdateUserOrganizationsAsync(int userID, List<int> selectedOrganizationIds, string databasePassword, string databaseName)
    {
      var factoryManager = new FactoryManager(databasePassword, databaseName);
      await factoryManager.GetUserOrganizationManager().UpdateUserOrganizationsAsync(userID, selectedOrganizationIds, databasePassword, databaseName);
    }

    public async Task<int> DeleteByOrganizationIdAsync(int organizationId, string databasePassword, string databaseName)
    {
      var factoryManager = new FactoryManager(databasePassword, databaseName);
      return await factoryManager.GetUserOrganizationManager().DeleteByOrganizationIdAsync(organizationId, databasePassword, databaseName);
    }
  }
}