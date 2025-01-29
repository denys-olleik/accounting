using Accounting.Business;
using static Accounting.Business.Claim;

namespace Accounting.Service
{
  public abstract class BaseService
  {
    protected readonly string _databaseName;
    protected readonly string _databasePassword;

    public BaseService() : this(
      DatabaseThing.DatabaseConstants.Database,
      CustomClaimTypeConstants.Password)
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