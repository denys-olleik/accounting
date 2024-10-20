﻿using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IDatabaseManager : IGenericRepository<DatabaseThing, string>
  {
    Task<DatabaseThing> CreateDatabase(string tenantId);
    Task ResetDatabaseAsync();
  }
}