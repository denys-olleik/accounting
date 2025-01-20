using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class PaymentInstructionService
  {
    private readonly string _databaseName;
    private readonly string _databasePassword;

    public PaymentInstructionService(string databasePassword, string databaseName = DatabaseThing.DatabaseConstants.Database)
    {
      _databaseName = databaseName;
      _databasePassword = databasePassword;
    }

    public Task CreateAsync(PaymentInstruction paymentInstruction)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return factoryManager.GetPaymentInstructionManager()
          .CreateAsync(paymentInstruction);
    }

    public Task<List<PaymentInstruction>> GetPaymentInstructionsAsync(int organizationId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return factoryManager.GetPaymentInstructionManager()
          .GetAllAsync(organizationId);
    }
  }
}
