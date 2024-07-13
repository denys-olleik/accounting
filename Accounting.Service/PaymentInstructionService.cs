using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
    public class PaymentInstructionService
    {
        public Task CreateAsync(PaymentInstruction paymentInstruction)
        {
            FactoryManager factoryManager = new FactoryManager();
            return factoryManager.GetPaymentInstructionManager()
                .CreateAsync(paymentInstruction);
        }

        public Task<List<PaymentInstruction>> GetPaymentInstructionsAsync(int organizationId)
        {
            FactoryManager factoryManager = new FactoryManager();
            return factoryManager.GetPaymentInstructionManager()
                .GetAllAsync(organizationId);
        }
    }
}