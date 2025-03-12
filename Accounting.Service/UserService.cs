using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class UserService : BaseService
  {
    public UserService() : base()
    {

    }

    public UserService(
      string databaseName, 
      string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<User> AddUserAsync(string? email, string? firstName, string? lastName, string? password)
    {
      throw new NotImplementedException();
    }

    public async Task<User> CreateAsync(User user, string databaseName, string databasePassword)
    {
      var factoryManager = new FactoryManager(databaseName, databasePassword);
      return await factoryManager.GetUserManager().CreateAsync(user);
    }

    public async Task<User> CreateAsync(User user)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetUserManager().CreateAsync(user);
    }

    public async Task<List<User>> GetAllAsync(int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetUserManager().GetAllAsync(organizationId);
    }

    public async Task<User> GetAsync(int userId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetUserManager().GetAsync(userId);
    }

    public async Task<(User, Tenant)> GetFirstOfAnyTenantAsync(string email)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetUserManager().GetFirstOfAnyTenantAsync(email);
    }

    public Task<int> UpdatePasswordAllTenantsAsync(string email, string password)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return factoryManager.GetUserManager().UpdatePasswordAllTenantsAsync(email, password);
    }

    public async Task<User> GetAsync(string email)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetUserManager().GetAsync(email);
    }

    public async Task<int> DeleteAsync(int userId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetUserManager().DeleteAsync(userId);
    }

    public async Task<(List<User> users, int? nextPageNumber)> GetAllAsync(
      int page, 
      int pageSize)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetUserManager().GetAllAsync(page, pageSize);
    }

    public async Task<List<User>> GetFilteredAsync(string search)
    {
      FactoryManager factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetUserManager().GetFilteredAsync(search);
    }

    public async Task<bool> IsUserInUseAsync(int userId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetUserManager().IsUserInUseAsync(userId, organizationId);
    }
  }
}