using Accounting.Business;
using Accounting.Common;
using static Accounting.Business.Claim;

namespace Accounting.Service
{
  public abstract class BaseService
  {
    protected readonly string _databaseName;
    protected readonly string _databasePassword;

    public BaseService() : this(
      DatabaseThing.DatabaseConstants.DatabaseName,
      ConfigurationSingleton.Instance.DatabasePassword)
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