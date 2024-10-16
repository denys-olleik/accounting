using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class ApplicationSettingsService
  {
    public async Task<ApplicationSetting> GetAsync(string key)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetApplicationSettingManager().GetAsync(key);
    }
  }
}