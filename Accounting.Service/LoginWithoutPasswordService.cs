using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class LoginWithoutPasswordService : BaseService
  {
    public LoginWithoutPasswordService() : base()
    {
      
    }

    public LoginWithoutPasswordService(
      string databaseName,
      string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<LoginWithoutPassword> CreateAsync(string email)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetLoginWithoutPasswordManager().CreateAsync(email);
    }

    public async Task<LoginWithoutPassword> GetAsync(string email)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetLoginWithoutPasswordManager().GetAsync(email);
    }

    public async Task DeleteAsync(LoginWithoutPassword loginWithoutPassword)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      await factoryManager.GetLoginWithoutPasswordManager().DeleteAsync(loginWithoutPassword);
    }

    public async Task<int> DeleteAsync(string? email)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetLoginWithoutPasswordManager().DeleteAsync(email);
    }
  }
}