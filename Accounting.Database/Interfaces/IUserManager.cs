using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IUserManager : IGenericRepository<User, int>
  {
    Task<List<User>> GetAllAsync(int organizationId);
    Task<User> GetAsync(int userId);
    Task<User> GetAsync(string email, bool searchTenants);
    Task<int> UpdatePasswordAsync(int userId, string passwordHash);
    Task<User> CreateAsync(User entity, string databaseName);
    Task<User> GetAsync(string email, int tenantId);
    Task<int> UpdatePasswordAllTenantsAsync(string email, string password);
  }
}