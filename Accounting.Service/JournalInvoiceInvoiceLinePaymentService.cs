using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class JournalInvoiceInvoiceLinePaymentService
  {
    public async Task<JournalInvoiceInvoiceLinePayment> CreateAsync(JournalInvoiceInvoiceLinePayment ledgerContext)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetJournalInvoiceInvoiceLinePaymentManager().CreateAsync(ledgerContext);
    }

    public async Task<List<JournalInvoiceInvoiceLinePayment>> GetAllAsync(int paymentId, bool getReversedEntries)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetJournalInvoiceInvoiceLinePaymentManager().GetAllAsync(paymentId, getReversedEntries);
    }

    public async Task<List<JournalInvoiceInvoiceLinePayment>> GetAllAsync(int invoicePaymentId, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetJournalInvoiceInvoiceLinePaymentManager().GetAllAsync(invoicePaymentId, organizationId);
    }
  }
}