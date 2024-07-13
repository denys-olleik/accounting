using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IDatabaseManager : IGenericRepository<DatabaseThing, int>
  {
    Task ResetDatabaseAsync();
  }
}