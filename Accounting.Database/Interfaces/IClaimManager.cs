using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IClaimManager : IGenericRepository<Claim, int>
  {
    Task<int> CreateClaimAsync(int userId, string claimType, string claimValue, int organizationId, int createdById);
    Task<int> DeleteRoles(int userID, int organizationId);
    Task<List<Claim>> GetAllAsync(int userID, int organizationId, string claimType);
    Task<Claim> GetAsync(int userId, string databaseName, string inRole);
    Task<int> GetUserCountWithRoleAsync(string role, int currentOrganizationId);
  }
}