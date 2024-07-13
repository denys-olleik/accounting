using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IAddressManager : IGenericRepository<Address, int>
  {
    Task<int> DeleteAsync(int businessEntityId);
    Task<List<Address>?> GetAllAsync(int businessEntityId);
    Task<Address> GetAsync(int selectedAddressId);
  }
}