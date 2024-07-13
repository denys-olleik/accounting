using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
    public class AddressService
    {
        public async Task<Address> CreateAsync(Address address)
        {
            FactoryManager factoryManager = new FactoryManager();
            return await factoryManager.GetAddressManager().CreateAsync(address);
        }

        public async Task<int> DeleteAsync(int businessEntityId)
        {
            FactoryManager factoryManager = new FactoryManager();
            return await factoryManager.GetAddressManager().DeleteAsync(businessEntityId);
        }

        public async Task<Address> GetAsync(int selectedAddressId)
        {
            FactoryManager factoryManager = new FactoryManager();
            return await factoryManager.GetAddressManager().GetAsync(selectedAddressId);
        }

        public async Task<List<Address>?> GetByAsync(int businessEntityId)
        {
            FactoryManager factoryManager = new FactoryManager();
            return await factoryManager.GetAddressManager().GetAllAsync(businessEntityId);
        }
    }
}