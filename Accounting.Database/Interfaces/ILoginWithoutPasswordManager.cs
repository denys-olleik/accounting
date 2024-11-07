using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface ILoginWithoutPasswordManager : IGenericRepository<LoginWithoutPassword, int>
  {
    Task<LoginWithoutPassword> CreateAsync(string email);
    Task<int> DeleteAsync(LoginWithoutPassword loginWithoutPassword);
    Task<int> DeleteAsync(string? email);
    Task<LoginWithoutPassword> GetAsync(string email);
  }
}