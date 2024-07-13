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

        public async Task<User> GetByEmailAsync(string email)
        {
            FactoryManager factoryManager = new FactoryManager();
            return await factoryManager.GetUserManager().GetByEmailAsync(email);
        }

        public async Task<int> UpdatePasswordAsync(int userId, string passwordHash)
        {
            FactoryManager factoryManager = new FactoryManager();
            return await factoryManager.GetUserManager().UpdatePasswordAsync(userId, passwordHash);
        }
    }
}