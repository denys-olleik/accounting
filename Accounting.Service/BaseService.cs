using Accounting.Business;

namespace Accounting.Service
{
  public abstract class BaseService
  {
    protected readonly string _databaseName;
    protected readonly string _databasePassword;

    public BaseService() : this(
      DatabaseThing.DatabaseConstants.Database, 
      "password")
    {

    }

    protected BaseService(
      string databaseName, 
      string databasePassword)
    {
      _databaseName = databaseName;
      _databasePassword = databasePassword;
    }
  }
}