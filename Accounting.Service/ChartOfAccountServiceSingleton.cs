using Accounting.Business;
using System.Collections.Concurrent;

namespace Accounting.Service
{
  public sealed class ChartOfAccountServiceSingleton
  {
    private static readonly ConcurrentDictionary<int, Lazy<Task<ChartOfAccountServiceSingleton>>> _instances
        = new ConcurrentDictionary<int, Lazy<Task<ChartOfAccountServiceSingleton>>>();

    public static async Task<ChartOfAccountServiceSingleton> InstanceAsync(int organizationId)
    {
      var lazy = _instances.GetOrAdd(organizationId,
          new Lazy<Task<ChartOfAccountServiceSingleton>>(() => InitializeAsync(organizationId)));
      return await lazy.Value;
    }

    public List<ChartOfAccount> Accounts { get; private set; }
    private readonly int _organizationId;

    private ChartOfAccountServiceSingleton(int organizationId)
    {
      _organizationId = organizationId;
      Accounts = new List<ChartOfAccount>();
    }

    private static async Task<ChartOfAccountServiceSingleton> InitializeAsync(int organizationId)
    {
      var instance = new ChartOfAccountServiceSingleton(organizationId);
      ChartOfAccountService chartOfAccountService = new ChartOfAccountService();
      instance.Accounts = await chartOfAccountService.GetAllAsync(organizationId);
      return instance;
    }
  }
}