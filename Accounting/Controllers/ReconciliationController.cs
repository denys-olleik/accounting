using CsvHelper.Configuration;
using CsvHelper;
using Accounting.Business;
using Accounting.Common;
using Accounting.CustomAttributes;
using Accounting.Models.ReconciliationViewModels;
using Accounting.Service;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Transactions;
using static Accounting.Models.ReconciliationViewModels.ReconciliationsViewModel;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("rec")]
  public class ReconciliationController : BaseController
  {
    private readonly ReconciliationTransactionService _reconciliationTransactionService;
    private readonly ReconciliationService _reconciliationService;
    private readonly ReconciliationAttachmentService _reconciliationAttachmentService;

    public ReconciliationController(
        ReconciliationTransactionService reconciliationTransactionService,
        ReconciliationService reconciliationService,
        ReconciliationAttachmentService reconciliationAttachmentService)
    {
      _reconciliationTransactionService = reconciliationTransactionService;
      _reconciliationService = reconciliationService;
      _reconciliationAttachmentService = reconciliationAttachmentService;
    }

    [Route("reconciliations")]
    [HttpGet]
    public async Task<IActionResult> Reconciliations()
    {
      List<Reconciliation> reconciliations
          = await _reconciliationService.GetAllDescendingAsync(20, GetOrganizationId());

      var model = new ReconciliationsViewModel();
      model.Reconciliations = new List<ReconciliationViewModel>();

      foreach (var reconciliation in reconciliations)
      {
        var reconciliationViewModel = new ReconciliationViewModel
        {
          ID = reconciliation.ReconciliationID,
          Status = reconciliation.Status,
          OriginalFileName = Path.GetFileName(reconciliation.ReconciliationAttachment.OriginalFileName),
          Created = reconciliation.Created,
          CreatedById = reconciliation.CreatedById,
          OrganizationId = reconciliation.OrganizationId
        };

        model.Reconciliations.Add(reconciliationViewModel);
      }

      return View(model);
    }

    [Route("reconciliation-details")]
    [HttpGet]
    public async Task<IActionResult> ReconciliationDetails(int id, int page = 1, int pageSize = 2)
    {
      var reconciliation = await _reconciliationService.GetByIdAsync(id, GetOrganizationId());
      // var transactions
      var rba = await _reconciliationAttachmentService.GetByReconciliationIdAsync(id, GetOrganizationId());

      var model = new ReconciliationDetailsViewModel
      {
        Page = page,
        PageSize = pageSize,

        ReconciliationId = reconciliation.ReconciliationID,
        Status = reconciliation.Status,
        OriginalFileName = rba.OriginalFileName,
        Created = reconciliation.Created,
        CreatedById = reconciliation.CreatedById,
        OrganizationId = reconciliation.OrganizationId
      };

      return View(model);
    }

    [Route("finalize")]
    [HttpPost]
    public async Task<IActionResult> Finalize(int reconciliationId)
    {
      var reconciliation = await _reconciliationService.GetByIdAsync(reconciliationId, GetOrganizationId());
      var transactions = await _reconciliationTransactionService.GetAllByReconciliationIdAsync(reconciliationId, GetOrganizationId());

      return RedirectToAction("Index", "Home");
    }

    [Route("map-import-columns")]
    [HttpGet]
    public async Task<IActionResult> MapImportColumns(List<string> columnNames, int reconciliationAttachment)
    {
      var model = new MapImportColumnsViewModel
      {
        ColumnNames = columnNames,
        ReconciliationAttachmentId = reconciliationAttachment
      };

      return View(model);
    }

    [Route("map-import-columns")]
    [HttpPost]
    public async Task<IActionResult> MapImportColumns(MapImportColumnsViewModel model)
    {
      var reconciliationTransactions = new List<ReconciliationTransaction>();

      var reconciliation = new Reconciliation
      {
        Status = Reconciliation.Statuses.Pending,
        CreatedById = GetUserId(),
        OrganizationId = GetOrganizationId(),
      };

      var ra = await _reconciliationAttachmentService.GetAsync(model.ReconciliationAttachmentId, GetOrganizationId());

      using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        var createdReconciliation = await _reconciliationService.CreateAsync(reconciliation);

        using (var reader = new StreamReader(ra.FilePath!))
        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
          var records = csv.GetRecords<dynamic>().ToList();
          foreach (var record in records)
          {
            var importData = new ReconciliationTransaction
            {
              ReconciliationId = createdReconciliation.ReconciliationID,
              Status = ReconciliationTransaction.ImportStatuses.Pending,
              CreatedById = GetUserId(),
              OrganizationId = GetOrganizationId()
            };

            foreach (var mapping in model.SelectedColumns)
            {
              var csvColumnName = mapping.Key;
              var dbColumnName = mapping.Value;

              var value = ((IDictionary<string, object>)record)[csvColumnName].ToString();
              switch (dbColumnName)
              {
                case "TransactionDate":
                  importData.TransactionDate = DateTime.Parse(value);
                  break;
                case "PostedDate":
                  importData.PostedDate = DateTime.Parse(value);
                  break;
                case "Description":
                  importData.Description = value;
                  break;
                case "Amount":
                  importData.Amount = Decimal.Parse(value);
                  break;
                case "Category":
                  importData.Category = value;
                  break;
              }
            }
            reconciliationTransactions.Add(importData);
          }
        }

        await _reconciliationTransactionService.ImportAsync(reconciliationTransactions);

        FileInfo fileInfo = new FileInfo(ra.FilePath!);
        string destinationPath = fileInfo.MoveToDirectory(ConfigurationSingleton.Instance.PermPath);

        await _reconciliationAttachmentService.UpdateFilePathAsync(ra.ReconciliationAttachmentID, destinationPath, GetOrganizationId());
        await _reconciliationAttachmentService.UpdateReconciliationIdAsync(ra.ReconciliationAttachmentID, createdReconciliation.ReconciliationID, GetOrganizationId());

        scope.Complete();
      }

      return RedirectToAction("Index", "Home");
    }

    [Route("import")]
    [HttpGet]
    public async Task<IActionResult> Import()
    {
      return View();
    }

    [Route("import")]
    [HttpPost]
    public async Task<IActionResult> Import(IFormFile file)
    {
      if (file == null || file.Length == 0)
      {
        return View("Error", "No file provided");
      }

      List<string> columnNames = new List<string>();
      string filePath;

      var commonFile = new Common.File
      {
        FileName = file.FileName,
        Stream = file.OpenReadStream()
      };

      filePath = await commonFile.SaveFile(ConfigurationSingleton.Instance.TempPath);

      var ra = await _reconciliationAttachmentService.CreateAsync(new ReconciliationAttachment
      {
        OriginalFileName = file.FileName,
        FilePath = filePath,
        CreatedById = GetUserId(),
        OrganizationId = GetOrganizationId(),
      });

      using (var reader = new StreamReader(file.OpenReadStream()))
      using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
      {
        csv.Read();
        csv.ReadHeader();

        columnNames = csv.Context.Reader.HeaderRecord!.ToList();
      }

      return RedirectToAction("MapImportColumns", new { columnNames, ReconciliationAttachment = ra.ReconciliationAttachmentID });
    }
  }
}