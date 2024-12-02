using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class AddressService
  {
    private readonly string _databaseName;

    public AddressService(string databaseName = DatabaseThing.DatabaseConstants.Database)
    {
      _databaseName = databaseName;
    }

    public async Task<Address> CreateAsync(Address address)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetAddressManager().CreateAsync(address);
    }

    public async Task<int> DeleteAsync(int businessEntityId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetAddressManager().DeleteAsync(businessEntityId);
    }

    public async Task<Address> GetAsync(int selectedAddressId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetAddressManager().GetAsync(selectedAddressId);
    }

    public async Task<List<Address>?> GetByAsync(int businessEntityId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetAddressManager().GetAllAsync(businessEntityId);
    }
  }
}