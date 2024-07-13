using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
    public class PaymentTermsService
    {
        public async Task<PaymentTerm> CreatePaymentTermAsync(PaymentTerm paymentTerm)
        {
            FactoryManager factoryManager = new FactoryManager();
            return await factoryManager.GetPaymentTermManager().CreateAsync(paymentTerm);
        }

        public async Task<List<PaymentTerm>> GetAllAsync()
        {
            FactoryManager factoryManager = new FactoryManager();
            return await factoryManager.GetPaymentTermManager().GetAllAsync();
        }

        public async Task<PaymentTerm?> GetAsync(int paymentTermId)
        {
            FactoryManager factoryManager = new FactoryManager();
            return await factoryManager.GetPaymentTermManager().GetAsync(paymentTermId);
        }
    }
}