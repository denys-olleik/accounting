using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class PaymentInstructionService : BaseService
  {
    public PaymentInstructionService() : base()
    {

    }

    public PaymentInstructionService(
      string databaseName,
      string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public Task CreateAsync(PaymentInstruction paymentInstruction)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return factoryManager.GetPaymentInstructionManager()
          .CreateAsync(paymentInstruction);
    }

    public Task<List<PaymentInstruction>> GetPaymentInstructionsAsync(int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return factoryManager.GetPaymentInstructionManager()
          .GetAllAsync(organizationId);
    }
  }
}