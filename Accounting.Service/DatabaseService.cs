﻿using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class DatabaseService : BaseService
  {
    public DatabaseService() : base()
    {

    }

    public DatabaseService(
      string databaseName, 
      string databasePassword) : base(databaseName, databasePassword)
    {
    }

    public async Task ResetDatabase()
    {
      var factoryManager = new FactoryManager(null, _databasePassword);
      await factoryManager.GetDatabaseManager().ResetDatabaseAsync();
    }

    public async Task<DatabaseThing> CreateDatabaseAsync(string tenantId)
    {
      var factoryManager = new FactoryManager(null, _databasePassword);
      return await factoryManager.GetDatabaseManager().CreateDatabase(tenantId);
    }

    public async Task RunSQLScript(string script, string databaseName)
    {
      var factoryManager = new FactoryManager(null, _databasePassword);
      await factoryManager.GetDatabaseManager().RunSQLScript(script, databaseName);
    }

    public async Task DeleteAsync(string databaseName)
    {
      var factoryManager = new FactoryManager(null, _databasePassword);
      await factoryManager.GetDatabaseManager().DeleteAsync(databaseName);
    }

    public async Task<string> BackupDatabaseAsync(string databaseName)
    {
      var factoryManager = new FactoryManager(null, _databasePassword);
      return await factoryManager.GetDatabaseManager().BackupDatabaseAsync(databaseName);
    }

    public async Task RestoreDatabase(string databaseName, Common.File file, int tenantId)
    {
      var factoryManager = new FactoryManager(null, _databasePassword);
      await factoryManager.GetDatabaseManager().RestoreDatabase(databaseName, file);

      // Update tenant record with localhost database password
      var managerFactory = new FactoryManager(null, _databasePassword);
      var tenantManager = managerFactory.GetTenantManager();
      await tenantManager.UpdateTenantDatabasePassword(tenantId, _databasePassword);
    }
  }
}