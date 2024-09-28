using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.Account;
using Accounting.Service;
using Accounting.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("a")]
  public class AccountController
    : BaseController
  {
    private readonly AccountService _accountService;
    private readonly JournalService _journalService;

    public AccountController(
      AccountService accountService, 
      JournalService journalService)
    {
      _accountService = accountService;
      _journalService = journalService;
    }

    [Route("accounts")]
    [HttpGet]
    public IActionResult Accounts()
    {
      return View();
    }

    [Route("create/{parentAccountId?}")]
    [HttpGet]
    public async Task<IActionResult> Create(int? parentAccountId)
    {
      Account parentAccount = null;

      if (parentAccountId.HasValue)
      {
        parentAccount = await _accountService.GetAsync(parentAccountId.Value, GetOrganizationId());
        if (parentAccount == null)
          return NotFound();
      }

      var model = new CreateAccountViewModel();
      model.AvailableAccountTypes = Account.AccountTypeConstants.All.ToList();

      if (parentAccount != null)
      {
        model.ParentAccountId = parentAccountId;
        model.ParentAccount = new CreateAccountViewModel.AccountViewModel()
        {
          AccountID = parentAccount!.AccountID,
          Name = parentAccount!.Name
        };
      }

      return View(model);
    }

    [Route("create/{parentAccountId?}")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateAccountViewModel model)
    {
      CreateAccountViewModelValidator validator = new CreateAccountViewModelValidator(_accountService);
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        model.AvailableAccountTypes = Account.AccountTypeConstants.All.ToList();

        return View(model);
      }

      Account account = new Account
      {
        Name = model.AccountName,
        Type = model.SelectedAccountType,
        ParentAccountId = model.ParentAccountId,
        InvoiceCreationForCredit = model.ShowInInvoiceCreationDropDownForCredit,
        InvoiceCreationForDebit = model.ShowInInvoiceCreationDropDownForDebit,
        ReceiptOfPaymentForCredit = model.ShowInReceiptOfPaymentDropDownForCredit,
        ReceiptOfPaymentForDebit = model.ShowInReceiptOfPaymentDropDownForDebit,
        ReconciliationExpense = model.ReconciliationExpense,
        ReconciliationLiabilitiesAndAssets = model.ReconciliationLiabilitiesAndAssets,
        OrganizationId = GetOrganizationId(),
        CreatedById = GetUserId()
      };

      await _accountService.CreateAsync(account);

      return RedirectToAction("Accounts");
    }

    [Route("update/{accountId}")]
    [HttpGet]
    public async Task<IActionResult> Update(int accountId)
    {
      Account account = await _accountService.GetAsync(accountId, GetOrganizationId());
      if (account == null)
        return NotFound();

      Account? parentAccount = null;

      if (account.ParentAccountId.HasValue)
        parentAccount = await _accountService.GetAsync(account.ParentAccountId!.Value, GetOrganizationId());

      UpdateAccountViewModel model = new UpdateAccountViewModel()
      {
        AccountID = account.AccountID,
        AccountName = account.Name,
        SelectedAccountType = account.Type,
        ShowInInvoiceCreationDropDownForCredit = account.InvoiceCreationForCredit,
        ShowInInvoiceCreationDropDownForDebit = account.InvoiceCreationForDebit,
        ShowInReceiptOfPaymentDropDownForCredit = account.ReceiptOfPaymentForCredit,
        ShowInReceiptOfPaymentDropDownForDebit = account.ReceiptOfPaymentForDebit,
        ReconciliationExpense = account.ReconciliationExpense,
        ReconciliationLiabilitiesAndAssets = account.ReconciliationLiabilitiesAndAssets,
        AvailableAccountTypes = Account.AccountTypeConstants.All.ToList()
      };

      if (parentAccount != null)
      {
        model.ParentAccountId = parentAccount.AccountID;
        model.ParentAccount = new UpdateAccountViewModel.AccountViewModel()
        {
          AccountID = parentAccount.AccountID,
          Name = parentAccount.Name
        };
      }

      return View(model);
    }

    [Route("update/{accountId}")]
    [HttpPost]
    public async Task<IActionResult> Update(int accountId, UpdateAccountViewModel model)
    {
      UpdateAccountViewModelValidator validator = new UpdateAccountViewModelValidator(_accountService, _journalService, GetOrganizationId());
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        model.AvailableAccountTypes = Account.AccountTypeConstants.All.ToList();

        if (model.ParentAccountId.HasValue)
        {
          var parentAccount = await _accountService.GetAsync(model.ParentAccountId.Value, GetOrganizationId());
          if (parentAccount != null)
          {
            model.ParentAccount = new UpdateAccountViewModel.AccountViewModel()
            {
              AccountID = parentAccount.AccountID,
              Name = parentAccount.Name
            };
          }
        }

        return View(model);
      }

      Account account = await _accountService.GetAsync(model.AccountID, GetOrganizationId());
      if (account == null)
        return NotFound();

      account.Name = model.AccountName;
      account.Type = model.SelectedAccountType;
      account.InvoiceCreationForCredit = model.ShowInInvoiceCreationDropDownForCredit;
      account.InvoiceCreationForDebit = model.ShowInInvoiceCreationDropDownForDebit;
      account.ReceiptOfPaymentForCredit = model.ShowInReceiptOfPaymentDropDownForCredit;
      account.ReceiptOfPaymentForDebit = model.ShowInReceiptOfPaymentDropDownForDebit;
      account.ReconciliationExpense = model.ReconciliationExpense;
      account.ReconciliationLiabilitiesAndAssets = model.ReconciliationLiabilitiesAndAssets;

      await _accountService.UpdateAsync(account);

      return RedirectToAction("Accounts");
    }
  }

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
      List<Account> accounts = await _accountService.GetAllHierachicalAsync(organizationId, true);

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
        JournalEntryCount = account.JournalEntryCount,
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