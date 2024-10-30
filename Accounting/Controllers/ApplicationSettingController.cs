using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.ApplicationSettingViewModels;
using Accounting.Service;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("application-settings")]
  public class ApplicationSettingController : BaseController
  {
    private readonly ApplicationSettingService _applicationSettingService;

    public ApplicationSettingController(ApplicationSettingService applicationSettingService)
    {
      _applicationSettingService = applicationSettingService;
    }

    [Route("application-settings")]
    public async Task<IActionResult> ApplicationSettings()
    {
      List<ApplicationSetting> applicationSettings 
        = await _applicationSettingService.GetAllAsync();

      ApplicationSettingsViewModel model = new ApplicationSettingsViewModel();
      model.ApplicationSettings = applicationSettings.Select(applicationSetting => new ApplicationSettingViewModel
      {
        ApplicationSettingID = applicationSetting.ApplicationSettingID,
        Key = applicationSetting.Key,
        Value = applicationSetting.Value
      }).ToList();

      return View(model);
    }
  }
}

namespace Accounting.Models.ApplicationSettingViewModels
{
  public class ApplicationSettingsViewModel
  {
    public List<ApplicationSettingViewModel>? ApplicationSettings { get; set; }
  }

  public class ApplicationSettingViewModel
  {
    public int ApplicationSettingID { get; set; }
    public string? Key { get; set; }
    public string? Value { get; set; }
  }
}