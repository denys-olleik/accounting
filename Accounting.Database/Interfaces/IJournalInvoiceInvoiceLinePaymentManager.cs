using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IJournalInvoiceInvoiceLinePaymentManager : IGenericRepository<JournalInvoiceInvoiceLinePayment, int>
  {
    Task<List<JournalInvoiceInvoiceLinePayment>> GetAllAsync(int paymentId, bool getReversedEntries);
    Task<List<JournalInvoiceInvoiceLinePayment>> GetAllAsync(int invoicePaymentId, int organizationId);
    Task<List<JournalInvoiceInvoiceLinePayment>> GetAllByPaymentIdAsync(int paymentId, int organizationId);
    Task<List<JournalInvoiceInvoiceLinePayment>?> GetAllByInvoiceIdAsync(int invoiceId, int organizationId, bool includeReversedEntries = false);
    Task<List<JournalInvoiceInvoiceLinePayment>> GetLastTransactionsAsync(int paymentID, int organizationId, bool loadChildren);
  }
}