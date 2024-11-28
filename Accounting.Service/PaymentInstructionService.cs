using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class PaymentInstructionService
  {
    private readonly string _databaseName;

    public PaymentInstructionService(string databaseName)
    {
      _databaseName = databaseName;
    }

    public Task CreateAsync(PaymentInstruction paymentInstruction)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return factoryManager.GetPaymentInstructionManager()
          .CreateAsync(paymentInstruction);
    }

    public Task<List<PaymentInstruction>> GetPaymentInstructionsAsync(int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return factoryManager.GetPaymentInstructionManager()
          .GetAllAsync(organizationId);
    }
  }
}
