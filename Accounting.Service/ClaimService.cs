using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class ClaimService : BaseService
  {
    public ClaimService() : base() { }
    public ClaimService(string databaseName, string databasePassword)
      : base(databaseName, databasePassword) { }

    public async Task<Claim> GetAsync(int userID, string databaseName, string inRole, int tenantID)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      var claimManager = factoryManager.GetClaimManager();
      return await claimManager.GetAsync(userID, databaseName, inRole, tenantID);
    }
  }
}