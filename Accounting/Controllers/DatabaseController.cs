using Accounting.Business;
using Accounting.Common;
using Accounting.CustomAttributes;
using Accounting.Models.DatabaseViewModels;
using Accounting.Service;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("database")]
  public class DatabaseController : BaseController
  {
    [Route("import/{tenantId}")]
    [HttpGet]
    public IActionResult Import(string tenantId)
    {
      return View();
    }

    [Route("import/{tenantId}")]
    [HttpPost]
    public async Task<IActionResult> Import(string tenantId, DatabaseImportViewModel model)
    {
      throw new NotImplementedException("Import");

      var tenant = await new TenantService().GetAsync(int.Parse(tenantId));
      if (tenant == null)
      {
        ValidationResult result = new ValidationResult();
        result.Errors.Add(new ValidationFailure("Tenant", "Tenant not found."));
        model.ValidationResult = result;
        return View(model);
      }

      if (model.DatabaseBackup == null || model.DatabaseBackup.Length == 0)
      {
        ValidationResult result = new ValidationResult();
        result.Errors.Add(new ValidationFailure("File", "File is required."));
        model.ValidationResult = result;
        return View(model);
      }

      Common.File file = new()
      {
        FileName = model.DatabaseBackup.FileName,
        Stream = model.DatabaseBackup.OpenReadStream()
      };

      DatabaseService databaseService = new();
      await databaseService.RestoreDatabase(tenant.DatabaseName!, file);

      return RedirectToAction("Tenants", "Tenant");
    }
  }

  [AuthorizeWithOrganizationId]
  [Route("api/database")]
  [ApiController]
  public class DatabaseApiController : BaseController
  {
    private readonly DatabaseService _databaseService;

    public DatabaseApiController(
      RequestContext requestContext,
      DatabaseService databaseService)
    {
      _databaseService = new(
        requestContext.DatabaseName,
        requestContext.DatabasePassword);
    }

    [HttpGet("download-backup/{tenantId}")]
    public async Task<IActionResult> DownloadBackup(string tenantId)
    {
      TenantService _tenantService = new();
      Tenant tenant = await _tenantService.GetAsync(int.Parse(tenantId));
      if (tenant == null)
      {
        return NotFound();
      }

      string path = await _databaseService.BackupDatabaseAsync(tenant.DatabaseName!);

      var fileName = Path.GetFileName(path);
      var contentType = "application/octet-stream";
      var fileBytes = await System.IO.File.ReadAllBytesAsync(path);

      return File(fileBytes, contentType, fileName);
    }
  }
}