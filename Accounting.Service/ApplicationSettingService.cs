using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class ApplicationSettingService
  {
    public async Task<List<ApplicationSetting>> GetAllAsync()
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetApplicationSettingManager().GetAllAsync();
    }

    public async Task<ApplicationSetting> GetAsync(string key)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetApplicationSettingManager().GetAsync(key);
    }

    public async Task UpsertAsync(ApplicationSetting applicationSetting)
    {
      FactoryManager factoryManager = new FactoryManager();
      await factoryManager.GetApplicationSettingManager().UpsertAsync(applicationSetting);
    }
  }
}