using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class DatabaseService
  {
    private readonly string _databasePassword;

    public DatabaseService(string databasePassword = "password")
    {
      _databasePassword = databasePassword;
    }

    public async Task ResetDatabase()
    {
      var factoryManager = new FactoryManager(_databasePassword, null);
      await factoryManager.GetDatabaseManager().ResetDatabaseAsync();
    }

    public async Task<DatabaseThing> CreateDatabaseAsync(string tenantId)
    {
      var factoryManager = new FactoryManager(_databasePassword, null);
      return await factoryManager.GetDatabaseManager().CreateDatabase(tenantId);
    }

    public async Task RunSQLScript(string script, string databaseName)
    {
      var factoryManager = new FactoryManager(_databasePassword, null);
      await factoryManager.GetDatabaseManager().RunSQLScript(script, databaseName);
    }

    public async Task DeleteAsync(string databaseName)
    {
      var factoryManager = new FactoryManager(_databasePassword, null);
      await factoryManager.GetDatabaseManager().DeleteAsync(databaseName);
    }
  }
}