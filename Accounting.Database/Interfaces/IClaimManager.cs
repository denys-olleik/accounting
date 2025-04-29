using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IClaimManager : IGenericRepository<Claim, int>
  {
    Task<List<Claim>> GetAllAsync(int userID, int organizationId, string claimType);
    Task<Claim> GetAsync(int userId, string databaseName, string inRole, int tenantID);
  }
}