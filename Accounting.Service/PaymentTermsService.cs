using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class PaymentTermsService : BaseService
  {
    public PaymentTermsService() : base()
    {

    }

    public PaymentTermsService(
      string databaseName,
      string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<PaymentTerm> CreatePaymentTermAsync(PaymentTerm paymentTerm)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetPaymentTermManager().CreateAsync(paymentTerm);
    }

    public async Task<List<PaymentTerm>> GetAllAsync()
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetPaymentTermManager().GetAllAsync();
    }

    public async Task<PaymentTerm?> GetAsync(int paymentTermId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetPaymentTermManager().GetAsync(paymentTermId);
    }
  }
}