using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.ApplicationSettingViewModels;
using Accounting.Service;
using Accounting.Validators;
using FluentValidation;
using FluentValidation.Results;
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

    [Route("upsert")]
    [HttpGet]
    public IActionResult Upsert()
    {
      return View();
    }

    [Route("upsert")]
    [HttpPost]
    public async Task<IActionResult> Upsert(CreateApplicationSettingViewModel model)
    {
      CreateApplicationSettingViewModelValidator validator 
        = new CreateApplicationSettingViewModelValidator(_applicationSettingService);
      ValidationResult result = await validator.ValidateAsync(model);

      if (!result.IsValid)
      {
        model.ValidationResult = result;
        return View(model);
      }

      ApplicationSetting applicationSetting = new ApplicationSetting
      {
        Key = model.Key,
        Value = model.Value
      };

      await _applicationSettingService.UpsertAsync(applicationSetting);

      return RedirectToAction("ApplicationSettings");
    }
  }
}

namespace Accounting.Validators
{
  public class CreateApplicationSettingViewModelValidator : AbstractValidator<CreateApplicationSettingViewModel>
  {
    private readonly ApplicationSettingService _applicationSettingService;

    public CreateApplicationSettingViewModelValidator(ApplicationSettingService applicationSettingService)
    {
      _applicationSettingService = applicationSettingService;

      RuleFor(x => x.Key)
        .NotEmpty()
        .WithMessage("Key is required.");

      RuleFor(x => x.Value)
        .NotEmpty()
        .WithMessage("Value is required.");
    }
  }
}

namespace Accounting.Models.ApplicationSettingViewModels
{
  public class ApplicationSettingsViewModel
  {
    public List<ApplicationSettingViewModel>? ApplicationSettings { get; set; }
  }

  public class CreateApplicationSettingViewModel
  {
    public string? Key { get; set; }
    public string? Value { get; set; }

    public ValidationResult ValidationResult { get; set; }
  }

  public class ApplicationSettingViewModel
  {
    public int ApplicationSettingID { get; set; }

    private string? key;
    public string? Key
    {
      get => key;
      set => key = value?.Trim().ToLower();
    }

    private string? value;
    public string? Value
    {
      get => value;
      set => this.value = value?.Trim().ToLower();
    }
  }
}