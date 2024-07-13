using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IReconiliationAttachmentManager : IGenericRepository<ReconciliationAttachment, int>
  {
    Task<ReconciliationAttachment> GetAsync(int reconciliationAttachmentId, int organizationId);
    Task<ReconciliationAttachment> GetByReconciliationIdAsync(int reconciliationId, int organizationId);
    Task<int> UpdateFilePathAsync(int id, string destinationPath, int organizationId);
    Task<int> UpdateReconciliationIdAsync(int id, int reconciliationId, int organizationId);
  }
}