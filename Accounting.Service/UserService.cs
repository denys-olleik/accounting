using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class UserService
  {
    public async Task<User> AddUserAsync(string? email, string? firstName, string? lastName, string? password)
    {
      throw new NotImplementedException();
    }

    public async Task<User> CreateAsync(User user)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetUserManager().CreateAsync(user);
    }

    public async Task<User> CreateAsync(User user, string databaseName)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetUserManager().CreateAsync(user, databaseName);
    }

    public async Task<List<User>> GetAllAsync(int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetUserManager().GetAllAsync(organizationId);
    }

    public async Task<User> GetAsync(int userId, int tenantId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetUserManager().GetAsync(userId, tenantId);
    }

    public async Task<(User, Tenant)> GetFirstOfAnyTenantAsync(string email)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetUserManager().GetFirstOfAnyTenantAsync(email);
    }

    public async Task<User> GetAsync(string email, int tenantId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetUserManager().GetAsync(email, tenantId);
    }

    public Task<int> UpdatePasswordAllTenantsAsync(string email, string password)
    {
      FactoryManager factoryManager = new FactoryManager();
      return factoryManager.GetUserManager().UpdatePasswordAllTenantsAsync(email, password);
    }

    public async Task<User> GetAsync(string email, string databaseName)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetUserManager().GetAsync(email, databaseName);
    }

    public async Task<User> GetAsync(int userId, string databaseName)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetUserManager().GetAsync(userId, databaseName);
    }
  }
}