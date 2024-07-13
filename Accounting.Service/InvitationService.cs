using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
    public class InvitationService
    {
        public async Task<Invitation> CreatAsync(Invitation invitation)
        {
            FactoryManager factoryManager = new FactoryManager();
            return await factoryManager.GetInvitationManager().CreateAsync(invitation);
        }

        public async Task<int> DeleteAsync(Guid guid)
        {
            FactoryManager factoryManager = new FactoryManager();
            return await factoryManager.GetInvitationManager().DeleteAsync(guid);
        }

        public async Task<Invitation> GetAsync(Guid guid)
        {
            FactoryManager factoryManager = new FactoryManager();
            return await factoryManager.GetInvitationManager().GetAsync(guid);
        }
    }
}