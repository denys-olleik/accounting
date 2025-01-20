using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class ReconciliationService
  {
    private readonly string _databaseName;
    private readonly string _databasePassword;

    public ReconciliationService(string databasePassword = "password", string databaseName = DatabaseThing.DatabaseConstants.Database)
    {
      _databaseName = databaseName;
      _databasePassword = databasePassword;
    }

    public async Task<Reconciliation> CreateAsync(Reconciliation reconciliation)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.ReconciliationManager().CreateAsync(reconciliation);
    }

    public async Task<List<Reconciliation>> GetAllDescendingAsync(int top, int organizationId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      var reconciliations = await factoryManager.ReconciliationManager().GetAllDescendingAsync(top, organizationId);
      return reconciliations.OrderByDescending(x => x.ReconciliationID).ToList();
    }

    public async Task<Reconciliation> GetByIdAsync(int id, int organizationId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.ReconciliationManager().GetByIdAsync(id, organizationId);
    }

    public async Task ProcessAsync(int reconciliationId, int organizationId)
    {
      var reconciliation = await GetByIdAsync(reconciliationId, organizationId);
      throw new NotImplementedException();
    }
  }
}