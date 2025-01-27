using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class AddressService : BaseService
  {
    public AddressService() : base()
    {
      
    }

    public AddressService(string databaseName, string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<Address> CreateAsync(Address address)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetAddressManager().CreateAsync(address);
    }

    public async Task<int> DeleteAsync(int businessEntityId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetAddressManager().DeleteAsync(businessEntityId);
    }

    public async Task<Address> GetAsync(int selectedAddressId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetAddressManager().GetAsync(selectedAddressId);
    }

    public async Task<List<Address>?> GetByAsync(int businessEntityId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetAddressManager().GetAllAsync(businessEntityId);
    }
  }
}