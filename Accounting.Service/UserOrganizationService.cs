using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
    public class UserOrganizationService
    {
        public async Task<List<Organization>> GetByUserIdAsync(int userId)
        {
           FactoryManager factoryManager = new FactoryManager();
            return await factoryManager.GetUserOrganizationManager().GetByUserIdAsync(userId);
        }

        public async Task<UserOrganization> GetAsync(int userId, int organizationId)
        {
            FactoryManager factoryManager = new FactoryManager();
            return await factoryManager.GetUserOrganizationManager().GetAsync(userId, organizationId);
        }

        public async Task<UserOrganization> CreateAsync(UserOrganization userOrganization)
        {
            FactoryManager factoryManager = new FactoryManager();
            return await factoryManager.GetUserOrganizationManager().CreateAsync(userOrganization);
        }
    }
}