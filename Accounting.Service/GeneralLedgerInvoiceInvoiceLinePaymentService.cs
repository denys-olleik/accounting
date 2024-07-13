using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class GeneralLedgerInvoiceInvoiceLinePaymentService
  {
    public async Task<GeneralLedgerInvoiceInvoiceLinePayment> CreateAsync(GeneralLedgerInvoiceInvoiceLinePayment ledgerContext)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetGeneralLedgerInvoiceInvoiceLinePaymentManager().CreateAsync(ledgerContext);
    }

    public async Task<List<GeneralLedgerInvoiceInvoiceLinePayment>> GetAllAsync(int paymentId, bool getReversedEntries)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetGeneralLedgerInvoiceInvoiceLinePaymentManager().GetAllAsync(paymentId, getReversedEntries);
    }

    public async Task<List<GeneralLedgerInvoiceInvoiceLinePayment>> GetAllAsync(int invoicePaymentId, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetGeneralLedgerInvoiceInvoiceLinePaymentManager().GetAllAsync(invoicePaymentId, organizationId);
    }
  }
}