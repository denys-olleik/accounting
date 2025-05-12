using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IDatabaseManager : IGenericRepository<DatabaseThing, string>
  {
    Task<string> BackupDatabaseAsync(string databaseName);
    Task<DatabaseThing> CreateDatabase(string tenantId);
    Task DeleteAsync(string databaseName);
    Task ResetDatabaseAsync();
    Task RestoreDatabase(string databaseName, Common.File file);
    Task RunSQLScript(string script, string databaseName);
  }
}