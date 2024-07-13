using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.ChartOfAccount;
using Accounting.Service;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [ApiController]
  [Route("api/coa")]
  public class ChartOfAccountApiController : BaseController
  {
    private readonly ChartOfAccountService _chartOfAccountService;

    public ChartOfAccountApiController(ChartOfAccountService chartOfAccountService)
    {
      _chartOfAccountService = chartOfAccountService;
    }

    [HttpGet("account-types")]
    public IActionResult GetChartOfAccountConstants()
    {
      return Ok(ChartOfAccount.AccountTypeConstants.All);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllChartOfAccounts()
    {
      var organizationId = GetOrganizationId();
      List<ChartOfAccount> accounts = await _chartOfAccountService.GetAllHierachicalAsync(organizationId);

      List<ChartOfAccountViewModel> accountsViewmodel = accounts
          .Where(account => account.ParentChartOfAccountId == null)
          .Select(ConvertToViewModel)
          .ToList();

      return Ok(accountsViewmodel);
    }

    [HttpGet("all-reconciliation-expense")]
    public async Task<IActionResult> GetAllReconciliationExpenseChartOfAccounts()
    {
      var organizationId = GetOrganizationId();
      List<ChartOfAccount> accounts = await _chartOfAccountService.GetAllReconciliationExpenseAsync(organizationId);

      List<ChartOfAccountViewModel> accountsViewmodel = accounts.Select(x => new ChartOfAccountViewModel
      {
        ChartOfAccountID = x.ChartOfAccountID,
        Name = x.Name,
        Type = x.Type,
        InvoiceCreationForCredit = x.InvoiceCreationForCredit,
        InvoiceCreationForDebit = x.InvoiceCreationForDebit,
        ReceiptOfPaymentForCredit = x.ReceiptOfPaymentForCredit,
        ReceiptOfPaymentForDebit = x.ReceiptOfPaymentForDebit,
        Created = x.Created,
        ParentChartOfAccountId = x.ParentChartOfAccountId,
        CreatedById = x.CreatedById,
        Children = new List<ChartOfAccountViewModel>()
      }).ToList();

      return Ok(accountsViewmodel);
    }

    [HttpGet("all-reconciliation-liabilities-and-assets")]
    public async Task<IActionResult> GetAllReconciliationLiabilitiesAndAssetsChartOfAccounts()
    {
      var organizationId = GetOrganizationId();
      List<ChartOfAccount> accounts = await _chartOfAccountService.GetAllReconciliationLiabilitiesAndAssetsAsync(organizationId);

      List<ChartOfAccountViewModel> accountsViewmodel = accounts.Select(x => new ChartOfAccountViewModel
      {
        ChartOfAccountID = x.ChartOfAccountID,
        Name = x.Name,
        Type = x.Type,
        InvoiceCreationForCredit = x.InvoiceCreationForCredit,
        InvoiceCreationForDebit = x.InvoiceCreationForDebit,
        ReceiptOfPaymentForCredit = x.ReceiptOfPaymentForCredit,
        ReceiptOfPaymentForDebit = x.ReceiptOfPaymentForDebit,
        Created = x.Created,
        ParentChartOfAccountId = x.ParentChartOfAccountId,
        CreatedById = x.CreatedById,
        Children = new List<ChartOfAccountViewModel>()
      }).ToList();

      return Ok(accountsViewmodel);
    }

    private ChartOfAccountViewModel ConvertToViewModel(ChartOfAccount account)
    {
      var viewModel = new ChartOfAccountViewModel
      {
        ChartOfAccountID = account.ChartOfAccountID,
        Name = account.Name,
        Type = account.Type,
        InvoiceCreationForCredit = account.InvoiceCreationForCredit,
        InvoiceCreationForDebit = account.InvoiceCreationForDebit,
        ReceiptOfPaymentForCredit = account.ReceiptOfPaymentForCredit,
        ReceiptOfPaymentForDebit = account.ReceiptOfPaymentForDebit,
        Created = account.Created,
        ParentChartOfAccountId = account.ParentChartOfAccountId,
        CreatedById = account.CreatedById,
        Children = new List<ChartOfAccountViewModel>()
      };

      if (account.Children != null)
      {
        foreach (var child in account.Children)
        {
          viewModel.Children.Add(ConvertToViewModel(child));
        }
      }

      return viewModel;
    }
  }
}