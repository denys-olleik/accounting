using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IInvoiceAttachmentManager : IGenericRepository<InvoiceAttachment, int>
  {
    Task<int> DeleteAsync(int invoiceAttachmentID, int invoiceID, int organizationId);
    Task<List<InvoiceAttachment>> GetAllAsync(int[] ids, int organizationId);
    Task<List<InvoiceAttachment>> GetAllAsync(int invoiceId, int organizationId);
    Task<InvoiceAttachment> GetAsync(int invoiceAttachmentId, int organizationId);
    Task<int> UpdateFilePathAsync(int id, string newPath, int organizationId);
    Task<int> UpdateInvoiceIdAsync(int invoiceAttachmentId, int invoiceId, int organizationId);
    Task<int> UpdatePrintOrderAsync(int id, int newPrintOrder, int userId, int organizationId);
  }
}