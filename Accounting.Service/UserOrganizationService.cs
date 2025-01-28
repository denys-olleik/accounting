using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class UserOrganizationService : BaseService
  {
    public UserOrganizationService() : base()
    {

    }

    public UserOrganizationService(
      string databaseName, 
      string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<List<Organization>> GetByUserIdAsync(int userId, string databaseName, string databasePassword)
    {
      var factoryManager = new FactoryManager(databaseName, databasePassword);
      return await factoryManager.GetUserOrganizationManager().GetByUserIdAsync(userId);
    }

    public async Task<UserOrganization> GetAsync(int userId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetUserOrganizationManager().GetAsync(userId, organizationId);
    }

    public async Task<UserOrganization> CreateAsync(UserOrganization userOrganization)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetUserOrganizationManager().CreateAsync(userOrganization);
    }

    public async Task CreateAsync(UserOrganization userOrganization, string databaseName, string databasePassword)
    {
      var factoryManager = new FactoryManager(databaseName, databasePassword);
      await factoryManager.GetUserOrganizationManager().CreateAsync(userOrganization, databaseName);
    }

    public async Task<List<UserOrganization>> GetAllAsync(int tenantId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetUserOrganizationManager().GetAllAsync(tenantId);
    }

    public async Task<List<(Organization Organization, Tenant? Tenant)>> GetByEmailAsync(string email, bool searchTenants)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetUserOrganizationManager().GetByEmailAsync(email, searchTenants);
    }

    public async Task<UserOrganization> GetByEmailAsync(string email, int? selectedOrganizationId, int tenantPublicId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetUserOrganizationManager().GetByEmailAsync(email, selectedOrganizationId, tenantPublicId);
    }

    public async Task<List<User>> GetUsersWithOrganizationsAsync(string databaseName, string databasePassword)
    {
      var factoryManager = new FactoryManager(databaseName, databasePassword);
      return await factoryManager.GetUserOrganizationManager().GetUsersWithOrganizationsAsync(databaseName);
    }

    public async Task<UserOrganization> CreateAsync(int userID, int organizationId, string databaseName = DatabaseThing.DatabaseConstants.Database, string databasePassword = "password")
    {
      var factoryManager = new FactoryManager(databaseName, databasePassword);
      return await factoryManager.GetUserOrganizationManager().CreateAsync(userID, organizationId, databaseName);
    }

    public async Task UpdateUserOrganizationsAsync(int userID, List<int> selectedOrganizationIds, string databaseName, string databasePassword)
    {
      var factoryManager = new FactoryManager(databaseName, databasePassword);
      await factoryManager.GetUserOrganizationManager().UpdateUserOrganizationsAsync(userID, selectedOrganizationIds, databasePassword, databaseName);
    }

    public async Task<int> DeleteByOrganizationIdAsync(int organizationId, string databaseName, string databasePassword)
    {
      var factoryManager = new FactoryManager(databaseName, databasePassword);
      return await factoryManager.GetUserOrganizationManager().DeleteByOrganizationIdAsync(organizationId, databasePassword, databaseName);
    }
  }
}