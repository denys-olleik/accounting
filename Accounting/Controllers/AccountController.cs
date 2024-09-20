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
    private readonly GeneralLedgerService _generalLedgerService;

    public AccountController(
      AccountService accountService, 
      GeneralLedgerService generalLedgerService)
    {
      _accountService = accountService;
      _generalLedgerService = generalLedgerService;
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
      UpdateAccountViewModelValidator validator = new UpdateAccountViewModelValidator(_accountService, _generalLedgerService, GetOrganizationId());
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
}