using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class DatabaseService
  {
    private readonly string _databaseName = DatabaseThing.DatabaseConstants.Database; 

    public async Task ResetDatabase()
    {
      var factoryManager = new FactoryManager(_databaseName);
      await factoryManager.GetDatabaseManager().ResetDatabaseAsync();
    }

    public async Task<DatabaseThing> CreateDatabaseAsync(string tenantId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetDatabaseManager().CreateDatabase(tenantId);
    }

    public async Task RunSQLScript(string script)
    {
      var factoryManager = new FactoryManager(_databaseName);
      await factoryManager.GetDatabaseManager().RunSQLScript(script, _databaseName);
    }

    public async Task DeleteAsync()
    {
      var factoryManager = new FactoryManager(_databaseName);
      await factoryManager.GetDatabaseManager().DeleteAsync(_databaseName);
    }
  }
}
