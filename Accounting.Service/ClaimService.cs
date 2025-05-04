using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class ClaimService : BaseService
  {
    public ClaimService() : base() { }
    public ClaimService(string databaseName, string databasePassword)
      : base(databaseName, databasePassword) { }

    public async Task<int> CreateRoleAsync(int userID, int organizationId, string role)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      var claimManager = factoryManager.GetClaimManager();
      return await claimManager.CreateClaimAsync(userID, Claim.CustomClaimTypeConstants.Role, role, organizationId, userID);
    }

    public async Task<Claim> GetAsync(int userId, string databaseName, string inRole)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      var claimManager = factoryManager.GetClaimManager();
      return await claimManager.GetAsync(userId, databaseName, inRole);
    }

    public async Task<int> GetUserCountWithRoleAsync(string role, int currentOrganizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      var claimManager = factoryManager.GetClaimManager();
      return await claimManager.GetUserCountWithRoleAsync(role, currentOrganizationId);
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

    public async Task<int> UpdateUserRolesAsync(int userID, List<string> selectedRoles, int organizationId, int createdById)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      var claimManager = factoryManager.GetClaimManager();

      await claimManager.DeleteRoles(userID, organizationId);

      int addedCount = 0;
      if (selectedRoles != null && selectedRoles.Count > 0)
      {
        foreach (var role in selectedRoles)
        {
          await claimManager.CreateClaimAsync(userID, Claim.CustomClaimTypeConstants.Role, role, organizationId, createdById);
          addedCount++;
        }
      }

      return addedCount;
    }
  }
}