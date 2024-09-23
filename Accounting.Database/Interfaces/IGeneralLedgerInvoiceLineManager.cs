using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IJournalInvoiceInvoiceLineManager : IGenericRepository<JournalInvoiceInvoiceLine, int>
  {
    Task<List<JournalInvoiceInvoiceLine>> GetAllAsync(int invoiceId, int organizationId, bool includeRemoved);
    Task<List<InvoiceLine>> GetByInvoiceIdAsync(int invoiceId, int organizationId, bool onlyCurrent = true);
    Task<List<JournalInvoiceInvoiceLine>> GetLastTransactionAsync(int invoiceLineID, int organizationId, bool loadChildren = false);
  }
}