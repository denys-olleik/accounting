using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class UserService
  {
    public async Task<User> CreateAsync(User user)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetUserManager().CreateAsync(user);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetUserManager().EmailExistsAsync(email);
    }

    public async Task<List<User>> GetAllAsync(int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetUserManager().GetAllAsync(organizationId);
    }

    public async Task<User> GetAsync(int userId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetUserManager().GetAsync(userId);
    }

    public async Task<User> GetAsync(string email, bool searchTenants)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetUserManager().GetAsync(email, searchTenants);
    }

    public async Task<int> UpdatePasswordAsync(int userId, string passwordHash)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetUserManager().UpdatePasswordAsync(userId, passwordHash);
    }
  }
}