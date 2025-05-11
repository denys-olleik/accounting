using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Service;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
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
      _databaseService = new (
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