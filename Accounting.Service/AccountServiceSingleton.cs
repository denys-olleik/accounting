using Accounting.Business;
using System.Collections.Concurrent;

namespace Accounting.Service
{
  public sealed class AccountServiceSingleton
  {
    private static readonly ConcurrentDictionary<int, Lazy<Task<AccountServiceSingleton>>> _instances
        = new ConcurrentDictionary<int, Lazy<Task<AccountServiceSingleton>>>();

    public static async Task<AccountServiceSingleton> InstanceAsync(int organizationId)
    {
      var lazy = _instances.GetOrAdd(organizationId,
          new Lazy<Task<AccountServiceSingleton>>(() => InitializeAsync(organizationId)));
      return await lazy.Value;
    }

    public List<Account> Accounts { get; private set; }
    private readonly int _organizationId;

    private AccountServiceSingleton(int organizationId)
    {
      _organizationId = organizationId;
      Accounts = new List<Account>();
    }

    private static async Task<AccountServiceSingleton> InitializeAsync(int organizationId)
    {
      var instance = new AccountServiceSingleton(organizationId);
      AccountService accountService = new AccountService();
      instance.Accounts = await accountService.GetAllAsync(organizationId);
      return instance;
    }
  }
}