using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IDatabaseManager : IGenericRepository<DatabaseThing, string>
  {
    Task<DatabaseThing> CreateDatabase(string tenantId);
    Task DeleteTenantDatabases();
    Task ResetDatabaseAsync();
    Task RunSQLScript(string script, string databaseName);
  }
}