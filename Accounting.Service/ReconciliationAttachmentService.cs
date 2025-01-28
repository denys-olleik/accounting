using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class ReconciliationAttachmentService : BaseService
  {
    public ReconciliationAttachmentService() : base()
    {
      
    }

    public ReconciliationAttachmentService(
      string databaseName,
      string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<ReconciliationAttachment> CreateAsync(ReconciliationAttachment attachment)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.ReconiliationAttachmentManager().CreateAsync(attachment);
    }

    public async Task<ReconciliationAttachment> GetAsync(int reconciliationAttachmentId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.ReconiliationAttachmentManager().GetAsync(reconciliationAttachmentId, organizationId);
    }

    public async Task<ReconciliationAttachment> GetByReconciliationIdAsync(int reconciliationId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.ReconiliationAttachmentManager().GetByReconciliationIdAsync(reconciliationId, organizationId);
    }

    public async Task<int> UpdateFilePathAsync(int id, string destinationPath, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.ReconiliationAttachmentManager().UpdateFilePathAsync(id, destinationPath, organizationId);
    }

    public async Task<int> UpdateReconciliationIdAsync(int id, int reconciliationId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.ReconiliationAttachmentManager().UpdateReconciliationIdAsync(id, reconciliationId, organizationId);
    }
  }
}