using Accounting.Business;
using Accounting.Common;
using Accounting.CustomAttributes;
using Accounting.Models.ReconciliationViewModels;
using Accounting.Service;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;
using static Accounting.Models.ReconciliationViewModels.GetReconciliationTransactionsViewModel;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [ApiController]
  [Route("api/rcl")]
  public class ReconciliationApiController : BaseController
  {
    private readonly ReconciliationTransactionService _reconciliationTransactionService;
    private readonly ReconciliationService _reconciliationService;
    private readonly GeneralLedgerService _generalLedgerService;
    private readonly GeneralLedgerReconciliationTransactionService _generalLedgerReconciliationTransactionService;
    private readonly ChartOfAccountService _chartOfAccountService;

    public ReconciliationApiController(
      ReconciliationTransactionService reconciliationTransactionService,
      ReconciliationService reconciliationService,
      GeneralLedgerReconciliationTransactionService generalLedgerExpenseService,
      ChartOfAccountService chartOfAccountService,
      GeneralLedgerService generalLedgerService)
    {
      _reconciliationTransactionService = reconciliationTransactionService;
      _reconciliationService = reconciliationService;
      _generalLedgerReconciliationTransactionService = generalLedgerExpenseService;
      _chartOfAccountService = chartOfAccountService;
      _generalLedgerService = generalLedgerService;
    }

    [HttpPost]
    [Route("update-reconciliation-transaction-instruction")]
    public async Task<IActionResult> UpdateReconciliationTransactionInstruction(UpdateReconciliationTransactionInstructionViewModel model)
    {
      ReconciliationTransaction reconciliationTransaction = await _reconciliationTransactionService.GetAsync(model.ReconciliationTransactionID);

      Guid transactionGuid = GuidExtensions.CreateSecureGuid();

      var chartOfAccountService = await ChartOfAccountServiceSingleton.InstanceAsync(GetOrganizationId());

      if (model.ReconciliationInstruction == ReconciliationTransaction.ReconciliationInstructions.Expense)
      {
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
          List<GeneralLedgerReconciliationTransaction> lastTransaction = await _generalLedgerReconciliationTransactionService.GetLastRelevantTransactionsAsync(reconciliationTransaction.ReconciliationTransactionID, GetOrganizationId(), true);

          var expenseAccount = chartOfAccountService.Accounts.Single(x => x.ChartOfAccountID == model.SelectedReconciliationExpenseAccountId);
          var liabilitiesOrAssetAccount = chartOfAccountService.Accounts.Single(x => x.ChartOfAccountID == model.SelectedReconciliationLiabilitiesAndAssetsAccountId);

          if (lastTransaction.Any() && !lastTransaction.Any(x => x.ReversedGeneralLedgerReconciliationTransactionId.HasValue))
          {
            foreach (var entry in lastTransaction)
            {
              GeneralLedger reversingGlEntry = await _generalLedgerService.CreateAsync(new GeneralLedger()
              {
                ChartOfAccountId = entry.GeneralLedger!.ChartOfAccountId,
                Debit = entry.GeneralLedger!.Credit,
                Credit = entry.GeneralLedger!.Debit,
                CreatedById = GetUserId(),
                OrganizationId = GetOrganizationId()
              });

              await _generalLedgerReconciliationTransactionService.CreateAsync(new GeneralLedgerReconciliationTransaction()
              {
                ReconciliationTransactionId = entry.ReconciliationTransactionId,
                GeneralLedgerId = reversingGlEntry.GeneralLedgerID,
                TransactionGuid = transactionGuid,
                ReversedGeneralLedgerReconciliationTransactionId = entry.GeneralLedgerReconciliationTransactionID,
                CreatedById = GetUserId(),
                OrganizationId = GetOrganizationId(),
              });
            }
          }
          GeneralLedger debit = await _generalLedgerService.CreateAsync(new GeneralLedger()
          {
            ChartOfAccountId = expenseAccount.ChartOfAccountID,
            Debit = reconciliationTransaction.Amount,
            Credit = 0,
            CreatedById = GetUserId(),
            OrganizationId = GetOrganizationId()
          });

          GeneralLedger credit = await _generalLedgerService.CreateAsync(new GeneralLedger()
          {
            ChartOfAccountId = liabilitiesOrAssetAccount.ChartOfAccountID,
            Debit = 0,
            Credit = reconciliationTransaction.Amount,
            CreatedById = GetUserId(),
            OrganizationId = GetOrganizationId()
          });

          await _generalLedgerReconciliationTransactionService.CreateAsync(new GeneralLedgerReconciliationTransaction()
          {
            ReconciliationTransactionId = reconciliationTransaction.ReconciliationTransactionID,
            GeneralLedgerId = debit.GeneralLedgerID,
            TransactionGuid = transactionGuid,
            CreatedById = GetUserId(),
            OrganizationId = GetOrganizationId(),
          });

          await _generalLedgerReconciliationTransactionService.CreateAsync(new GeneralLedgerReconciliationTransaction()
          {
            ReconciliationTransactionId = reconciliationTransaction.ReconciliationTransactionID,
            GeneralLedgerId = credit.GeneralLedgerID,
            TransactionGuid = transactionGuid,
            CreatedById = GetUserId(),
            OrganizationId = GetOrganizationId(),
          });

          await _reconciliationTransactionService
            .UpdateReconciliationTransactionInstructionAsync(
              model.ReconciliationTransactionID,
              ReconciliationTransaction.ReconciliationInstructions.Expense);
          await _reconciliationTransactionService.UpdateExpenseChartOfAccountIdAsync(model.ReconciliationTransactionID, model.SelectedReconciliationExpenseAccountId);
          await _reconciliationTransactionService.UpdateAssetOrLiabilityChartOfAccountIdAsync(model.ReconciliationTransactionID, model.SelectedReconciliationLiabilitiesAndAssetsAccountId);

          scope.Complete();
        }
      }
      else if (model.ReconciliationInstruction == ReconciliationTransaction.ReconciliationInstructions.Revenue)
      {
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
          // Assuming a Revenue entity and service similar to Expense
          //Revenue revenue = await _revenueService.GetAsync(reconciliationTransaction.ReconciliationTransactionID);

          //if (revenue == null)
          //{
          //  // Create a new Revenue record
          //  revenue = await _revenueService.CreateAsync(new Revenue()
          //  {
          //    // Set necessary properties, similar to Expense creation
          //  });
          //}
          //else
          //{
          //  // Update the existing Revenue record
          //  revenue.Amount = reconciliationTransaction.Amount;
          //  // Update other necessary properties
          //  await _revenueService.UpdateAsync(revenue);
          //}

          // Adjust ledger entries for Revenue
          // This will likely involve similar logic to the Expense case,
          // but with appropriate adjustments for revenue accounting

          // Finally, update the reconciliation transaction instruction
          await _reconciliationTransactionService
            .UpdateReconciliationTransactionInstructionAsync(
              model.ReconciliationTransactionID,
              ReconciliationTransaction.ReconciliationInstructions.Revenue);

          scope.Complete();
        }
      }
      else if (string.IsNullOrEmpty(model.ReconciliationInstruction))
      {
        throw new NotImplementedException();
      }

      reconciliationTransaction
        = await _reconciliationTransactionService.GetAsync(model.ReconciliationTransactionID);

      var result = new
      {
        reconciliationTransaction.ReconciliationTransactionID,
        reconciliationTransaction.ReconciliationInstruction,
      };

      return Ok(result);
    }

    [HttpPost]
    [Route("update-reconciliation-transaction-instruction-unset-expense")]
    public async Task<IActionResult> UpdateReconciliationTransactionInstructionUnsetExpense(UpdateReconciliationTransactionInstructionViewModel model)
    {
      ReconciliationTransaction reconciliationTransaction = await _reconciliationTransactionService.GetAsync(model.ReconciliationTransactionID);

      Guid transactionGuid = GuidExtensions.CreateSecureGuid();

      using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        List<GeneralLedgerReconciliationTransaction> lastTransaction = await _generalLedgerReconciliationTransactionService.GetLastRelevantTransactionsAsync(reconciliationTransaction.ReconciliationTransactionID, GetOrganizationId(), true);

        if (lastTransaction.Any() && !lastTransaction.Any(x => x.ReversedGeneralLedgerReconciliationTransactionId.HasValue))
        {
          foreach (var entry in lastTransaction)
          {
            GeneralLedger reversingGlEntry = await _generalLedgerService.CreateAsync(new GeneralLedger()
            {
              ChartOfAccountId = entry.GeneralLedger!.ChartOfAccountId,
              Debit = entry.GeneralLedger!.Credit,
              Credit = entry.GeneralLedger!.Debit,
              CreatedById = GetUserId(),
              OrganizationId = GetOrganizationId()
            });

            await _generalLedgerReconciliationTransactionService.CreateAsync(new GeneralLedgerReconciliationTransaction()
            {
              ReconciliationTransactionId = entry.ReconciliationTransactionId,
              GeneralLedgerId = reversingGlEntry.GeneralLedgerID,
              TransactionGuid = transactionGuid,
              ReversedGeneralLedgerReconciliationTransactionId = entry.GeneralLedgerReconciliationTransactionID,
              CreatedById = GetUserId(),
              OrganizationId = GetOrganizationId(),
            });
          }

          await _reconciliationTransactionService
            .UpdateReconciliationTransactionInstructionAsync(model.ReconciliationTransactionID, null);
          reconciliationTransaction
            = await _reconciliationTransactionService.GetAsync(model.ReconciliationTransactionID);
          scope.Complete();
        }

        var result = new { reconciliationTransaction.ReconciliationTransactionID, reconciliationTransaction.ReconciliationInstruction };
        return Ok(result);
      }
    }

    [HttpPost]
    [Route("process")]
    public async Task<IActionResult> Process(int reconciliationId)
    {
      using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        await _reconciliationService.ProcessAsync(reconciliationId, GetOrganizationId());

        scope.Complete();
      }

      return Ok();
    }

    [HttpGet("get-transactions")]
    public async Task<IActionResult> GetTransactions(int reconciliationId, int page = 1, int pageSize = 2)
    {
      var (reconciliationTransactions, nextPageNumber) = await _reconciliationTransactionService.GetReconciliationTransactionsAsync(reconciliationId, page, pageSize);

      GetReconciliationTransactionsViewModel vm = new GetReconciliationTransactionsViewModel()
      {
        ReconciliationTransactions = reconciliationTransactions.Select(rt => new ReconciliationTransactionViewModel
        {
          ReconciliationTransactionID = rt.ReconciliationTransactionID,
          Status = rt.Status,
          RawData = rt.RawData,
          ReconciliationInstruction = rt.ReconciliationInstruction,
          TransactionDate = rt.TransactionDate,
          PostedDate = rt.PostedDate,
          Description = rt.Description,
          Amount = rt.Amount,
          Category = rt.Category,
          Created = rt.Created,
          ReconciliationId = rt.ReconciliationId,
          CreatedById = rt.CreatedById,
          OrganizationId = rt.OrganizationId
        }).ToList(),
        Page = page,
        NextPage = nextPageNumber
      };

      return Ok(vm);
    }
  }
}