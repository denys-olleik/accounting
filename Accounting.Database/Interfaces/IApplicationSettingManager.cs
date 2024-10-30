using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IApplicationSettingManager : IGenericRepository<ApplicationSetting, int>
  {
    Task<List<ApplicationSetting>> GetAllAsync();
    Task<ApplicationSetting> GetAsync(string key);
  }
}