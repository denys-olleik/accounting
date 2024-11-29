using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class DatabaseService
  {
    public async Task ResetDatabase()
    {
      var factoryManager = new FactoryManager(null);
      await factoryManager.GetDatabaseManager().ResetDatabaseAsync();
    }

    public async Task<DatabaseThing> CreateDatabaseAsync(string tenantId)
    {
      var factoryManager = new FactoryManager(null);
      return await factoryManager.GetDatabaseManager().CreateDatabase(tenantId);
    }

    public async Task RunSQLScript(string script, string databaseName)
    {
      var factoryManager = new FactoryManager(null);
      await factoryManager.GetDatabaseManager().RunSQLScript(script, databaseName);
    }

    public async Task DeleteAsync(string databaseName)
    {
      var factoryManager = new FactoryManager(null);
      await factoryManager.GetDatabaseManager().DeleteAsync(databaseName);
    }
  }
}