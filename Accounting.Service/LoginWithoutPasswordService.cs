using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class LoginWithoutPasswordService
  {
    private readonly string _databaseName;

    public LoginWithoutPasswordService(string databaseName = DatabaseThing.DatabaseConstants.Database)
    {
      _databaseName = databaseName;
    }

    public async Task<LoginWithoutPassword> CreateAsync(string email)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetLoginWithoutPasswordManager().CreateAsync(email);
    }

    public async Task<LoginWithoutPassword> GetAsync(string email)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetLoginWithoutPasswordManager().GetAsync(email);
    }

    public async Task DeleteAsync(LoginWithoutPassword loginWithoutPassword)
    {
      var factoryManager = new FactoryManager(_databaseName);
      await factoryManager.GetLoginWithoutPasswordManager().DeleteAsync(loginWithoutPassword);
    }

    public async Task<int> DeleteAsync(string? email)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetLoginWithoutPasswordManager().DeleteAsync(email);
    }
  }
}