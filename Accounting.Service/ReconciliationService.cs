using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class ReconciliationService : BaseService
  {
    public ReconciliationService() : base()
    {

    }

    public ReconciliationService(
      string databaseName,
      string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<Reconciliation> CreateAsync(Reconciliation reconciliation)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.ReconciliationManager().CreateAsync(reconciliation);
    }

    public async Task<List<Reconciliation>> GetAllDescendingAsync(int top, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      var reconciliations = await factoryManager.ReconciliationManager().GetAllDescendingAsync(top, organizationId);
      return reconciliations.OrderByDescending(x => x.ReconciliationID).ToList();
    }

    public async Task<Reconciliation> GetByIdAsync(int id, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.ReconciliationManager().GetByIdAsync(id, organizationId);
    }

    public async Task ProcessAsync(int reconciliationId, int organizationId)
    {
      var reconciliation = await GetByIdAsync(reconciliationId, organizationId);
      throw new NotImplementedException();
    }
  }
}