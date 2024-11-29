using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class InvitationService
  {
    private readonly string _databaseName;

    public InvitationService(string databaseName = null)
    {
      _databaseName = databaseName;
    }

    public async Task<Invitation> CreatAsync(Invitation invitation)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvitationManager().CreateAsync(invitation);
    }

    public async Task<int> DeleteAsync(Guid guid)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvitationManager().DeleteAsync(guid);
    }

    public async Task<Invitation> GetAsync(Guid guid)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvitationManager().GetAsync(guid);
    }
  }
}