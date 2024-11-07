using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class LoginWithoutPasswordService
  {
    public async Task<LoginWithoutPassword> CreateAsync(string email)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetLoginWithoutPasswordManager().CreateAsync(email);
    }

    public async Task<LoginWithoutPassword> GetAsync(string email)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetLoginWithoutPasswordManager().GetAsync(email);
    }

    public async Task DeleteAsync(LoginWithoutPassword loginWithoutPassword)
    {
      FactoryManager factoryManager = new FactoryManager();
      await factoryManager.GetLoginWithoutPasswordManager().DeleteAsync(loginWithoutPassword);
    }

    public async Task<int> DeleteAsync(string? email)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetLoginWithoutPasswordManager().DeleteAsync(email);
    }
  }
}