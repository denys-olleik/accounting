using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IClaimManager : IGenericRepository<Claim, int>
  {
    Task<Claim> GetAsync(int userId, string databaseName, string inRole);
  }
}