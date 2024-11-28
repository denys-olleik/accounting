using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class UserService
  {
    private readonly string _databaseName;

    public UserService(string databaseName)
    {
      _databaseName = databaseName;
    }

    public async Task<User> AddUserAsync(string? email, string? firstName, string? lastName, string? password)
    {
      throw new NotImplementedException();
    }

    public async Task<User> CreateAsync(User user)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetUserManager().CreateAsync(user);
    }

    public async Task<User> CreateAsync(User user, string databaseName)
    {
      var factoryManager = new FactoryManager(databaseName);
      return await factoryManager.GetUserManager().CreateAsync(user, databaseName);
    }

    public async Task<List<User>> GetAllAsync(int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetUserManager().GetAllAsync(organizationId);
    }

    public async Task<User> GetAsync(int userId, string databaseName)
    {
      var factoryManager = new FactoryManager(databaseName);
      return await factoryManager.GetUserManager().GetAsync(userId, databaseName);
    }

    public async Task<(User, Tenant)> GetFirstOfAnyTenantAsync(string email)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetUserManager().GetFirstOfAnyTenantAsync(email);
    }

    public async Task<User> GetAsync(string email, int tenantId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetUserManager().GetAsync(email, tenantId);
    }

    public Task<int> UpdatePasswordAllTenantsAsync(string email, string password)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return factoryManager.GetUserManager().UpdatePasswordAllTenantsAsync(email, password);
    }

    public async Task<User> GetAsync(string email, string databaseName)
    {
      var factoryManager = new FactoryManager(databaseName);
      return await factoryManager.GetUserManager().GetAsync(email, databaseName);
    }

    public async Task<int> UpdateAsync(string email, string firstName, string lastName, string databaseName)
    {
      var factoryManager = new FactoryManager(databaseName);
      return await factoryManager.GetUserManager().UpdateAsync(email, firstName, lastName, databaseName);
    }
  }
}
