using Accounting.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Database.Interfaces
{
  public interface IApplicationSettingManager : IGenericRepository<ApplicationSetting, int>
  {
    Task<ApplicationSetting> GetAsync(string key);
  }
}