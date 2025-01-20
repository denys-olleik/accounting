using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class ReconciliationAttachmentService
  {
    private readonly string _databaseName;
    private readonly string _databasePassword;

    public ReconciliationAttachmentService(string databasePassword = "password", string databaseName = DatabaseThing.DatabaseConstants.Database)
    {
      _databaseName = databaseName;
      _databasePassword = databasePassword;
    }

    public async Task<ReconciliationAttachment> CreateAsync(ReconciliationAttachment attachment)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.ReconiliationAttachmentManager().CreateAsync(attachment);
    }

    public async Task<ReconciliationAttachment> GetAsync(int reconciliationAttachmentId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.ReconiliationAttachmentManager().GetAsync(reconciliationAttachmentId, organizationId);
    }

    public async Task<ReconciliationAttachment> GetByReconciliationIdAsync(int reconciliationId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.ReconiliationAttachmentManager().GetByReconciliationIdAsync(reconciliationId, organizationId);
    }

    public async Task<int> UpdateFilePathAsync(int id, string destinationPath, int organizationId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.ReconiliationAttachmentManager().UpdateFilePathAsync(id, destinationPath, organizationId);
    }

    public async Task<int> UpdateReconciliationIdAsync(int id, int reconciliationId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.ReconiliationAttachmentManager().UpdateReconciliationIdAsync(id, reconciliationId, organizationId);
    }
  }
}