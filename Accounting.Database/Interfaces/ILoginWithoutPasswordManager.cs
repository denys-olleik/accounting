using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface ILoginWithoutPasswordManager : IGenericRepository<LoginWithoutPassword, int>
  {
    Task<LoginWithoutPassword> CreateAsync(string email);
  }
}