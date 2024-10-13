using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IDatabaseManager : IGenericRepository<DatabaseThing, int>
  {
    Task<DatabaseThing> CreateDatabase(int tenantId);
    Task ResetDatabaseAsync();
  }
}