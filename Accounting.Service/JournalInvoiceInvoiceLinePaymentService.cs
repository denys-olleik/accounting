using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class JournalInvoiceInvoiceLinePaymentService
  {
    private readonly string _databaseName;

    public JournalInvoiceInvoiceLinePaymentService(string databaseName = DatabaseThing.DatabaseConstants.Database)
    {
      _databaseName = databaseName;
    }

    public async Task<JournalInvoiceInvoiceLinePayment> CreateAsync(JournalInvoiceInvoiceLinePayment ledgerContext)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetJournalInvoiceInvoiceLinePaymentManager().CreateAsync(ledgerContext);
    }

    public async Task<List<JournalInvoiceInvoiceLinePayment>> GetAllAsync(int paymentId, bool getReversedEntries)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetJournalInvoiceInvoiceLinePaymentManager().GetAllAsync(paymentId, getReversedEntries);
    }

    public async Task<List<JournalInvoiceInvoiceLinePayment>> GetAllAsync(int invoicePaymentId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetJournalInvoiceInvoiceLinePaymentManager().GetAllAsync(invoicePaymentId, organizationId);
    }
  }
}