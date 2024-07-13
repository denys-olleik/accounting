using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IGeneralLedgerInvoiceInvoiceLineManager : IGenericRepository<GeneralLedgerInvoiceInvoiceLine, int>
  {
    Task<List<GeneralLedgerInvoiceInvoiceLine>> GetAllAsync(int invoiceId, int organizationId, bool includeRemoved);
    Task<List<InvoiceLine>> GetByInvoiceIdAsync(int invoiceId, int organizationId, bool onlyCurrent = true);
    Task<List<GeneralLedgerInvoiceInvoiceLine>> GetLastTransactionAsync(int invoiceLineID, int organizationId, bool loadChildren = false);
  }
}