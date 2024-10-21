using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class DatabaseService
  {
    public async Task ResetDatabase()
    {
      FactoryManager factoryManager = new FactoryManager();
      await factoryManager.GetDatabaseManager().ResetDatabaseAsync();
    }

    public async Task<DatabaseThing> CreateDatabaseAsync(string tenantId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetDatabaseManager().CreateDatabase(tenantId);
    }

    public async Task DeleteTenantDatabases()
    {
      FactoryManager factoryManager = new FactoryManager();
      await factoryManager.GetDatabaseManager().DeleteTenantDatabases();
    }
  }
}