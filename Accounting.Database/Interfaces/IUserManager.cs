using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IUserManager : IGenericRepository<User, int>
  {
    Task<List<User>> GetAllAsync(int organizationId);
    Task<User> GetAsync(int userId);
    Task<(User, Tenant)> GetFirstOfAnyTenantAsync(string email);
    Task<User> CreateAsync(User entity);
    Task<int> UpdatePasswordAllTenantsAsync(string email, string password);
    Task<User> GetAsync(string email);
    Task<int> UpdateAsync(string email, string firstName, string lastName);
    Task<int> DeleteAsync(int userId);
  }
}