using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IInvoiceLineManager : IGenericRepository<InvoiceLine, int>
  {
    Task<int> DeleteByInvoiceIdAsync(int invoiceId, int organizationId);
    Task<List<InvoiceLine>> GetByInvoiceId(int invoiceId, int organizationId);
    Task<int> UpdateAsync(InvoiceLine line, int organizationId);
    Task<int> UpdateTitleAndDescription(List<InvoiceLine> invoiceLines, int invoiceID, int userId, int organizationId);
  }
}