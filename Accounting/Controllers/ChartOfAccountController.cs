using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.ChartOfAccount;
using Accounting.Service;
using Accounting.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("coa")]
  public class ChartOfAccountController
    : BaseController
  {
    private readonly ChartOfAccountService _chartOfAccountService;

    public ChartOfAccountController(
      ChartOfAccountService chartOfAccountService)
    {
      _chartOfAccountService = chartOfAccountService;
    }

    [Route("accounts")]
    [HttpGet]
    public IActionResult Accounts()
    {
      return View();
    }

    [Route("create/{parentChartOfAccountId?}")]
    [HttpGet]
    public async Task<IActionResult> Create(int? parentChartOfAccountId)
    {
      ChartOfAccount parentChartOfAccount = null;

      if (parentChartOfAccountId.HasValue)
      {
        parentChartOfAccount = await _chartOfAccountService.GetAsync(parentChartOfAccountId.Value, GetOrganizationId());
        if (parentChartOfAccount == null)
          return NotFound();
      }

      var model = new CreateChartOfAccountViewModel();
      model.AvailableAccountTypes = ChartOfAccount.AccountTypeConstants.All.ToList();

      if (parentChartOfAccount != null)
      {
        model.ParentChartOfAccountId = parentChartOfAccountId;
        model.ParentChartOfAccount = new CreateChartOfAccountViewModel.ChartOfAccountViewModel()
        {
          ChartOfAccountID = parentChartOfAccount!.ChartOfAccountID,
          Name = parentChartOfAccount!.Name
        };
      }

      return View(model);
    }

    [Route("create/{parentChartOfAccountId?}")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateChartOfAccountViewModel model)
    {
      CreateChartOfAccountViewModelValidator validator = new CreateChartOfAccountViewModelValidator();
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        model.AvailableAccountTypes = ChartOfAccount.AccountTypeConstants.All.ToList();

        return View(model);
      }

      ChartOfAccount account = new ChartOfAccount
      {
        Name = model.AccountName,
        Type = model.SelectedAccountType,
        ParentChartOfAccountId = model.ParentChartOfAccountId,
        InvoiceCreationForCredit = model.ShowInInvoiceCreationDropDownForCredit,
        InvoiceCreationForDebit = model.ShowInInvoiceCreationDropDownForDebit,
        ReceiptOfPaymentForCredit = model.ShowInReceiptOfPaymentDropDownForCredit,
        ReceiptOfPaymentForDebit = model.ShowInReceiptOfPaymentDropDownForDebit,
        ReconciliationExpense = model.ReconciliationExpense,
        ReconciliationLiabilitiesAndAssets = model.ReconciliationLiabilitiesAndAssets,
        OrganizationId = GetOrganizationId(),
        CreatedById = GetUserId()
      };

      await _chartOfAccountService.CreateAsync(account);

      return RedirectToAction("Accounts");
    }

    [Route("update/{chartOfAccountId}")]
    [HttpGet]
    public async Task<IActionResult> Update(int chartOfAccountId)
    {
      ChartOfAccount chartOfAccount = await _chartOfAccountService.GetAsync(chartOfAccountId, GetOrganizationId());
      if (chartOfAccount == null)
        return NotFound();

      ChartOfAccount? parentChartOfAccount = null;

      if (chartOfAccount.ParentChartOfAccountId.HasValue)
        parentChartOfAccount = await _chartOfAccountService.GetAsync(chartOfAccount.ParentChartOfAccountId!.Value, GetOrganizationId());

      UpdateChartOfAccountViewModel model = new UpdateChartOfAccountViewModel()
      {
        ChartOfAccountID = chartOfAccount.ChartOfAccountID,
        AccountName = chartOfAccount.Name,
        SelectedAccountType = chartOfAccount.Type,
        ShowInInvoiceCreationDropDownForCredit = chartOfAccount.InvoiceCreationForCredit,
        ShowInInvoiceCreationDropDownForDebit = chartOfAccount.InvoiceCreationForDebit,
        ShowInReceiptOfPaymentDropDownForCredit = chartOfAccount.ReceiptOfPaymentForCredit,
        ShowInReceiptOfPaymentDropDownForDebit = chartOfAccount.ReceiptOfPaymentForDebit,
        ReconciliationExpense = chartOfAccount.ReconciliationExpense,
        ReconciliationLiabilitiesAndAssets = chartOfAccount.ReconciliationLiabilitiesAndAssets,
        AvailableAccountTypes = ChartOfAccount.AccountTypeConstants.All.ToList()
      };

      if (parentChartOfAccount != null)
      {
        model.ParentChartOfAccountId = parentChartOfAccount.ChartOfAccountID;
        model.ParentChartOfAccount = new UpdateChartOfAccountViewModel.ChartOfAccountViewModel()
        {
          ChartOfAccountID = parentChartOfAccount.ChartOfAccountID,
          Name = parentChartOfAccount.Name
        };
      }

      return View(model);
    }

    [Route("update/{chartOfAccountId}")]
    [HttpPost]
    public async Task<IActionResult> Update(int chartOfAccountId, UpdateChartOfAccountViewModel model)
    {
      UpdateChartOfAccountViewModelValidator validator = new UpdateChartOfAccountViewModelValidator(_chartOfAccountService, GetOrganizationId());
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        model.AvailableAccountTypes = ChartOfAccount.AccountTypeConstants.All.ToList();

        return View(model);
      }

      ChartOfAccount chartOfAccount = await _chartOfAccountService.GetAsync(model.ChartOfAccountID, GetOrganizationId());
      if (chartOfAccount == null)
        return NotFound();

      chartOfAccount.Name = model.AccountName;
      chartOfAccount.Type = model.SelectedAccountType;
      chartOfAccount.InvoiceCreationForCredit = model.ShowInInvoiceCreationDropDownForCredit;
      chartOfAccount.InvoiceCreationForDebit = model.ShowInInvoiceCreationDropDownForDebit;
      chartOfAccount.ReceiptOfPaymentForCredit = model.ShowInReceiptOfPaymentDropDownForCredit;
      chartOfAccount.ReceiptOfPaymentForDebit = model.ShowInReceiptOfPaymentDropDownForDebit;
      chartOfAccount.ReconciliationExpense = model.ReconciliationExpense;
      chartOfAccount.ReconciliationLiabilitiesAndAssets = model.ReconciliationLiabilitiesAndAssets;

      await _chartOfAccountService.UpdateAsync(chartOfAccount);

      return RedirectToAction("Accounts");
    }
  }
}