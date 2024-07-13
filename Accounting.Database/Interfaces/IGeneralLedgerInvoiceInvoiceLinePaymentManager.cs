using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IGeneralLedgerInvoiceInvoiceLinePaymentManager : IGenericRepository<GeneralLedgerInvoiceInvoiceLinePayment, int>
  {
    Task<List<GeneralLedgerInvoiceInvoiceLinePayment>> GetAllAsync(int paymentId, bool getReversedEntries);
    Task<List<GeneralLedgerInvoiceInvoiceLinePayment>> GetAllAsync(int invoicePaymentId, int organizationId);
    Task<List<GeneralLedgerInvoiceInvoiceLinePayment>> GetAllByPaymentIdAsync(int paymentId, int organizationId);
    Task<List<GeneralLedgerInvoiceInvoiceLinePayment>?> GetAllByInvoiceIdAsync(int invoiceId, int organizationId, bool includeReversedEntries = false);
    Task<List<GeneralLedgerInvoiceInvoiceLinePayment>> GetLastTransactionsAsync(int paymentID, int organizationId, bool loadChildren);
  }
}