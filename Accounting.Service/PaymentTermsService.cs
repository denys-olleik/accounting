using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class PaymentTermsService
  {
    private readonly string _databaseName;
    private readonly string _databasePassword;

    public PaymentTermsService(string databasePassword = "password", string databaseName = DatabaseThing.DatabaseConstants.Database)
    {
      _databaseName = databaseName;
      _databasePassword = databasePassword;
    }

    public async Task<PaymentTerm> CreatePaymentTermAsync(PaymentTerm paymentTerm)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetPaymentTermManager().CreateAsync(paymentTerm);
    }

    public async Task<List<PaymentTerm>> GetAllAsync()
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetPaymentTermManager().GetAllAsync();
    }

    public async Task<PaymentTerm?> GetAsync(int paymentTermId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetPaymentTermManager().GetAsync(paymentTermId);
    }
  }
}