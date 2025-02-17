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
    private readonly JournalService _journalService;
    private readonly JournalReconciliationTransactionService _journalReconciliationTransactionService;
    private readonly AccountService _accountService;

    public ReconciliationApiController(
      ReconciliationTransactionService reconciliationTransactionService,
      ReconciliationService reconciliationService,
      JournalReconciliationTransactionService journalExpenseService,
      AccountService accountService,
      JournalService journalService,
      RequestContext requestContext)
    {
      _reconciliationTransactionService = new ReconciliationTransactionService(requestContext.DatabaseName, requestContext.DatabasePassword);
      _reconciliationService = new ReconciliationService(requestContext.DatabaseName, requestContext.DatabasePassword);
      _journalReconciliationTransactionService = new JournalReconciliationTransactionService(requestContext.DatabaseName, requestContext.DatabasePassword);
      _accountService = new AccountService(requestContext.DatabaseName, requestContext.DatabasePassword);
      _journalService = new JournalService(requestContext.DatabaseName, requestContext.DatabasePassword);
    }

    [HttpPost]
    [Route("update-reconciliation-transaction-instruction-unset-expense")]
    public async Task<IActionResult> UpdateReconciliationTransactionInstructionUnsetExpense(UpdateReconciliationTransactionInstructionViewModel model)
    {
      ReconciliationTransaction reconciliationTransaction = await _reconciliationTransactionService.GetAsync(model.ReconciliationTransactionID);

      Guid transactionGuid = GuidExtensions.CreateSecureGuid();

      using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        List<JournalReconciliationTransaction> lastTransaction = await _journalReconciliationTransactionService.GetLastRelevantTransactionsAsync(reconciliationTransaction.ReconciliationTransactionID, GetOrganizationId(), true);

        if (lastTransaction.Any() && !lastTransaction.Any(x => x.ReversedJournalReconciliationTransactionId.HasValue))
        {
          foreach (var entry in lastTransaction)
          {
            Journal reversingGlEntry = await _journalService.CreateAsync(new Journal()
            {
              AccountId = entry.Journal!.AccountId,
              Debit = entry.Journal!.Credit,
              Credit = entry.Journal!.Debit,
              CreatedById = GetUserId(),
              OrganizationId = GetOrganizationId()
            });

            await _journalReconciliationTransactionService.CreateAsync(new JournalReconciliationTransaction()
            {
              ReconciliationTransactionId = entry.ReconciliationTransactionId,
              JournalId = reversingGlEntry.JournalID,
              TransactionGuid = transactionGuid,
              ReversedJournalReconciliationTransactionId = entry.JournalReconciliationTransactionID,
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