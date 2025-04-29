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

    public async Task<List<string>> GetUserRolesAsync(int userID, int organizationId, string claimType)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      var claimManager = factoryManager.GetClaimManager();
      var claims = await claimManager.GetAllAsync(userID, organizationId, claimType);
      if (claims == null || claims.Count == 0)
      {
        return new List<string>();
      }
      else
      {
        return claims.Select(c => c.ClaimValue).ToList();
      }
    }
  }
}