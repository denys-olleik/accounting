using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class ReconciliationService
  {
    public async Task<Reconciliation> CreateAsync(Reconciliation reconciliation)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.ReconciliationManager().CreateAsync(reconciliation);
    }

    public async Task<List<Reconciliation>> GetAllDescendingAsync(int top, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      var reconciliations = await factoryManager.ReconciliationManager().GetAllDescendingAsync(top, organizationId);
      return reconciliations.OrderByDescending(x => x.ReconciliationID).ToList();
    }

    public async Task<Reconciliation> GetByIdAsync(int id, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.ReconciliationManager().GetByIdAsync(id, organizationId);
    }

    public async Task ProcessAsync(int reconciliationId, int organizationId)
    {
      Reconciliation reconciliation = await GetByIdAsync(reconciliationId, organizationId);



      throw new NotImplementedException();
    }
  }
}