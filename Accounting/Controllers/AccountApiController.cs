using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.Account;
using Accounting.Service;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [ApiController]
  [Route("api/a")]
  public class AccountApiController : BaseController
  {
    private readonly AccountService _accountService;

    public AccountApiController(AccountService accountService)
    {
      _accountService = accountService;
    }

    [HttpGet("account-types")]
    public IActionResult AccountConstants()
    {
      return Ok(Account.AccountTypeConstants.All);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllAccounts()
    {
      var organizationId = GetOrganizationId();
      List<Account> accounts = await _accountService.GetAllHierachicalAsync(organizationId);

      List<AccountViewModel> accountsViewmodel = accounts
          .Where(account => account.ParentAccountId == null)
          .Select(ConvertToViewModel)
          .ToList();

      return Ok(accountsViewmodel);
    }

    [HttpGet("all-reconciliation-expense")]
    public async Task<IActionResult> GetAllReconciliationExpenseAccounts()
    {
      var organizationId = GetOrganizationId();
      List<Account> accounts = await _accountService.GetAllReconciliationExpenseAccountsAsync(organizationId);

      List<AccountViewModel> accountsViewmodel = accounts.Select(x => new AccountViewModel
      {
        AccountID = x.AccountID,
        Name = x.Name,
        Type = x.Type,
        InvoiceCreationForCredit = x.InvoiceCreationForCredit,
        InvoiceCreationForDebit = x.InvoiceCreationForDebit,
        ReceiptOfPaymentForCredit = x.ReceiptOfPaymentForCredit,
        ReceiptOfPaymentForDebit = x.ReceiptOfPaymentForDebit,
        Created = x.Created,
        ParentAccountId = x.ParentAccountId,
        CreatedById = x.CreatedById,
        Children = new List<AccountViewModel>()
      }).ToList();

      return Ok(accountsViewmodel);
    }

    [HttpGet("all-reconciliation-liabilities-and-assets")]
    public async Task<IActionResult> GetAllReconciliationLiabilitiesAndAssetsAccounts()
    {
      var organizationId = GetOrganizationId();
      List<Account> accounts = await _accountService.GetAllReconciliationLiabilitiesAndAssetsAsync(organizationId);

      List<AccountViewModel> accountsViewmodel = accounts.Select(x => new AccountViewModel
      {
        AccountID = x.AccountID,
        Name = x.Name,
        Type = x.Type,
        InvoiceCreationForCredit = x.InvoiceCreationForCredit,
        InvoiceCreationForDebit = x.InvoiceCreationForDebit,
        ReceiptOfPaymentForCredit = x.ReceiptOfPaymentForCredit,
        ReceiptOfPaymentForDebit = x.ReceiptOfPaymentForDebit,
        Created = x.Created,
        ParentAccountId = x.ParentAccountId,
        CreatedById = x.CreatedById,
        Children = new List<AccountViewModel>()
      }).ToList();

      return Ok(accountsViewmodel);
    }

    private AccountViewModel ConvertToViewModel(Account account)
    {
      var viewModel = new AccountViewModel
      {
        AccountID = account.AccountID,
        Name = account.Name,
        Type = account.Type,
        InvoiceCreationForCredit = account.InvoiceCreationForCredit,
        InvoiceCreationForDebit = account.InvoiceCreationForDebit,
        ReceiptOfPaymentForCredit = account.ReceiptOfPaymentForCredit,
        ReceiptOfPaymentForDebit = account.ReceiptOfPaymentForDebit,
        Created = account.Created,
        ParentAccountId = account.ParentAccountId,
        CreatedById = account.CreatedById,
        Children = new List<AccountViewModel>()
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