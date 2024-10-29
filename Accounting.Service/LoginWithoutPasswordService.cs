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
  }
}