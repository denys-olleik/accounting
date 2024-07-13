using Accounting.Business;
using Accounting.Common;
using Accounting.CustomAttributes;
using Accounting.Models.AccountsReceivableViewModels;
using Accounting.Models.BusinessEntityViewModels;
using Accounting.Models.ChartOfAccount;
using Accounting.Models.InvoiceViewModels;
using Accounting.Models.PaymentViewModels;
using Accounting.Service;
using Accounting.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("ar")]
  public class AccountsReceivableController : BaseController
  {
    private readonly InvoiceService _invoiceService;
    private readonly ChartOfAccountService _chartOfAccountService;
    private readonly InvoiceLineService _invoiceLineService;
    private readonly GeneralLedgerService _generalLedgerService;
    private readonly BusinessEntityService _businessEntityService;
    private readonly PaymentService _paymentService;
    private readonly InvoiceInvoiceLinePaymentService _invoicePaymentService;
    private readonly GeneralLedgerInvoiceInvoiceLinePaymentService _generalLedgerInvoicePaymentService;
    private readonly GeneralLedgerInvoiceInvoiceLineService _generalLedgerInvoiceInvoiceLineService;

    public AccountsReceivableController(
        InvoiceService invoiceService,
        ChartOfAccountService chartOfAccountService,
        InvoiceLineService invoiceLineService,
        GeneralLedgerService generalLedgerService,
        BusinessEntityService businessEntityService,
        PaymentService paymentService,
        InvoiceInvoiceLinePaymentService invoicePaymentService,
        GeneralLedgerInvoiceInvoiceLinePaymentService generalLedgerPaymentService,
        GeneralLedgerInvoiceInvoiceLineService generalLedgerInvoiceInvoiceLineService)
    {
      _invoiceService = invoiceService;
      _chartOfAccountService = chartOfAccountService;
      _invoiceLineService = invoiceLineService;
      _generalLedgerService = generalLedgerService;
      _businessEntityService = businessEntityService;
      _paymentService = paymentService;
      _invoicePaymentService = invoicePaymentService;
      _generalLedgerInvoicePaymentService = generalLedgerPaymentService;
      _generalLedgerInvoiceInvoiceLineService = generalLedgerInvoiceInvoiceLineService;
    }

    [Route("receive-payment-for-invoice-ids")]
    [HttpGet]
    public async Task<IActionResult> ReceivePaymentForInvoiceIds(string invoiceIdsCsv)
    {
      List<Invoice> invoices = await FetchInvoices(invoiceIdsCsv);  
      List<ChartOfAccount> debitAccounts = await _chartOfAccountService.GetAccountOptionsForPaymentReceptionDebit(GetOrganizationId());

      var model = CreateReceivePaymentForInvoiceIdsViewModel(invoices, debitAccounts);
      return View(model);
    }

    [Route("receive-payment-for-invoice-ids")]
    [HttpPost]
    public async Task<IActionResult> ReceivePaymentForInvoiceIds(ReceivePaymentForInvoiceIdsViewModel model)
    {
      ValidationResult validationResult = await ValidateReceivePaymentForInvoiceIdsViewModel(model, _invoiceService);

      if (!validationResult.IsValid)
      {
        string invoiceIdsCsv = string.Join(",", model.Invoices.Select(i => i.InvoiceId.ToString()));
        List<Invoice> latestInvoices = await FetchInvoices(invoiceIdsCsv);
        List<ChartOfAccount> debitAccounts = await _chartOfAccountService.GetAccountOptionsForPaymentReceptionDebit(GetOrganizationId());

        model = RebuildInvalidModel(model, latestInvoices, debitAccounts, validationResult);
        return View(model);
      }

      using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        Payment payment = await _paymentService.CreateAsync(new Payment
        {
          ReferenceNumber = model.ReferenceNumber,
          Amount = model.PaymentTotal,
          CreatedById = GetUserId(),
          OrganizationId = GetOrganizationId()
        });

        Guid transactionGuid = GuidExtensions.CreateSecureGuid();

        foreach (var invoice in model.Invoices)
        {
          foreach (var invoiceLine in invoice.InvoiceLines!)
          {
            InvoiceInvoiceLinePayment ip = await _invoicePaymentService.CreateAsync(new InvoiceInvoiceLinePayment()
            {
              InvoiceId = invoice.InvoiceId,
              InvoiceLineId = invoiceLine.InvoiceLineID,
              PaymentId = payment.PaymentID,
              Amount = invoiceLine.AmountToReceive!.Value,
              CreatedById = GetUserId(),
              OrganizationId = GetOrganizationId(),
            });

            var debitAccount = await _chartOfAccountService.GetAsync(int.Parse(model.SelectedDebitAccountId!), GetOrganizationId());

            // Debit Entry
            var debitGlEntry = await _generalLedgerService.CreateAsync(new GeneralLedger()
            {
              ChartOfAccountId = debitAccount.ChartOfAccountID,
              Debit = invoiceLine.AmountToReceive,
              Credit = null,
              CreatedById = GetUserId(),
              OrganizationId = GetOrganizationId()
            });

            // Credit Entry
            var creditGlEntry = await _generalLedgerService.CreateAsync(new GeneralLedger()
            {
              ChartOfAccountId = invoiceLine.AssetsChartOfAccountId,
              Debit = null,
              Credit = invoiceLine.AmountToReceive,
              CreatedById = GetUserId(),
              OrganizationId = GetOrganizationId()
            });

            await _generalLedgerInvoicePaymentService.CreateAsync(new GeneralLedgerInvoiceInvoiceLinePayment()
            {
              GeneralLedgerId = debitGlEntry.GeneralLedgerID,
              InvoiceInvoiceLinePaymentId = ip.InvoiceInvoiceLinePaymentID,
              TransactionGuid = transactionGuid,
              CreatedById = GetUserId(),
              OrganizationId = GetOrganizationId()
            });

            await _generalLedgerInvoicePaymentService.CreateAsync(new GeneralLedgerInvoiceInvoiceLinePayment()
            {
              GeneralLedgerId = creditGlEntry.GeneralLedgerID,
              InvoiceInvoiceLinePaymentId = ip.InvoiceInvoiceLinePaymentID,
              TransactionGuid = transactionGuid,
              CreatedById = GetUserId(),
              OrganizationId = GetOrganizationId()
            });
          }

          await _invoiceService.ComputeAndUpdateInvoiceStatus(invoice.InvoiceId, GetOrganizationId());
          await _invoiceService.ComputeAndUpdateTotalAmountAndReceivedAmount(invoice.InvoiceId, GetOrganizationId());
        }

        scope.Complete();
      }

      return RedirectToAction("Invoices", "Invoice");
    }

    [Route("received-payments")]
    [HttpGet]
    public async Task<IActionResult> ReceivedPayments(int page = 1, int pageSize = 2)
    {
      var viewModel = new ReceivedPaymentsPaginatedViewModel
      {
        Page = page,
        PageSize = pageSize
      };

      return View(viewModel);
    }

    //[Route("void-invoice-payment")]
    //[HttpGet]
    //public async Task<IActionResult> VoidInvoicePayment(int id)
    //{
    //  InvoiceInvoiceLinePayment invoicePayment = await _invoicePaymentService.GetInvoicePaymentAsync(id, GetOrganizationId());
    //  invoicePayment.Invoice = await _invoiceService.GetAsync(invoicePayment.InvoiceId, GetOrganizationId());
    //  invoicePayment.Payment = await _paymentService.GetAsync(invoicePayment.PaymentId, GetOrganizationId());

    //  BusinessEntity businessEntity = await _businessEntityService.GetAsync(invoicePayment.Invoice!.BusinessEntityId, GetOrganizationId());

    //  VoidInvoicePaymentViewModel model = new VoidInvoicePaymentViewModel
    //  {
    //    ID = invoicePayment.InvoiceInvoiceLinePaymentID,
    //    Invoice = new Models.AccountsReceivableViewModels.VoidInvoicePaymentViewModels.InvoiceViewModel
    //    {
    //      ID = invoicePayment.InvoiceId,
    //      InvoiceNumber = invoicePayment.Invoice!.InvoiceNumber!.ToString()
    //    },
    //    Payment = new Models.AccountsReceivableViewModels.VoidInvoicePaymentViewModels.PaymentViewModel
    //    {
    //      ID = invoicePayment.PaymentId,
    //      ReferenceNumber = invoicePayment.Payment!.ReferenceNumber
    //    },
    //    BusinessEntity = new Models.AccountsReceivableViewModels.VoidInvoicePaymentViewModels.BusinessEntityViewModel
    //    {
    //      ID = invoicePayment.Invoice!.BusinessEntityId,
    //      DisplayName = Helpers.FormatBusinessEntityName(
    //              businessEntity.FirstName,
    //              businessEntity.LastName,
    //              businessEntity.CompanyName,
    //              businessEntity.CustomerType!)
    //    },
    //    Amount = invoicePayment.Amount
    //  };

    //  return View(model);
    //}

    //[Route("void-invoice-payment")]
    //[HttpPost]
    //public async Task<IActionResult> VoidInvoicePayment(VoidInvoicePaymentViewModel model)
    //{
    //  VoidInvoicePaymentViewModelValidator validator
    //      = new VoidInvoicePaymentViewModelValidator(_invoicePaymentService, GetOrganizationId());
    //  ValidationResult validationResult = await validator.ValidateAsync(model);

    //  if (!validationResult.IsValid)
    //  {
    //    model.ValidationResult = validationResult;
    //    return View(model);
    //  }

    //  using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
    //  {
    //    await _invoicePaymentService.VoidAsync(model.ID, model.VoidReason!, GetOrganizationId());
    //    List<GeneralLedgerInvoiceInvoiceLinePayment> invoicePayments = await _generalLedgerInvoicePaymentService.GetAllAsync(model.ID, GetOrganizationId());

    //    Guid transactionGuid = GuidExtensions.CreateSecureGuid();

    //    foreach (var invoicePayment in invoicePayments)
    //    {
    //      GeneralLedger gl = await _generalLedgerService.GetAsync(invoicePayment.GeneralLedgerId, GetOrganizationId());

    //      gl = await _generalLedgerService.CreateAsync(new GeneralLedger()
    //      {
    //        ChartOfAccountId = gl.ChartOfAccountId,
    //        Debit = gl.Credit,
    //        Credit = gl.Debit,
    //        CreatedById = GetUserId(),
    //        OrganizationId = GetOrganizationId()
    //      });

    //      await _generalLedgerInvoicePaymentService.CreateAsync(new GeneralLedgerInvoiceInvoiceLinePayment()
    //      {
    //        GeneralLedgerId = gl.GeneralLedgerID,
    //        InvoiceInvoiceLinePaymentId = model.ID,
    //        TransactionGuid = transactionGuid,
    //        ReversedGeneralLedgerInvoiceInvoiceLinePaymentId = invoicePayment.GeneralLedgerInvoiceInvoiceLinePaymentID,
    //        CreatedById = GetUserId(),
    //        OrganizationId = GetOrganizationId()
    //      });
    //    }

    //    await _invoiceService.ComputeAndUpdateTotalAmountAndReceivedAmount(model.Invoice.ID, GetOrganizationId());
    //    await _invoiceService.ComputeAndUpdateInvoiceStatus(model.Invoice.ID, GetOrganizationId());
    //    await _invoiceService.UpdateLastUpdated(model.Invoice.ID, GetOrganizationId());

    //    scope.Complete();
    //  }

    //  return View();
    //}

    private async Task<ValidationResult> ValidateReceivePaymentForInvoiceIdsViewModel(ReceivePaymentForInvoiceIdsViewModel model, InvoiceService invoiceService)
    {
      ReceivePaymentForInvoiceIdsViewModelValidator validator = new ReceivePaymentForInvoiceIdsViewModelValidator(GetOrganizationId(), invoiceService);
      return await validator.ValidateAsync(model);
    }

    private ReceivePaymentForInvoiceIdsViewModel RebuildInvalidModel(
        ReceivePaymentForInvoiceIdsViewModel model,
        List<Invoice> latestInvoices,
        List<ChartOfAccount> debitAccounts,
        ValidationResult validationResult)
    {
      var updatedModel = CreateReceivePaymentForInvoiceIdsViewModel(latestInvoices, debitAccounts);
      updatedModel.ValidationResult = validationResult;

      updatedModel.ReferenceNumber = model.ReferenceNumber;
      updatedModel.SelectedDebitAccountId = model.SelectedDebitAccountId;

      for (int i = 0; i < updatedModel.Invoices.Count; i++)
      {
        var originalInvoice = model.Invoices.FirstOrDefault(inv => inv.InvoiceId == updatedModel.Invoices[i].InvoiceId);
        if (originalInvoice != null)
        {
          updatedModel.Invoices[i].Amount = originalInvoice.Amount;
        }
      }

      return updatedModel;
    }

    private async Task<List<Invoice>> FetchInvoices(string invoiceIdsCsv)
    {
      List<Invoice> invoices = await _invoiceService.GetAsync(invoiceIdsCsv, GetOrganizationId());
      foreach (var invoice in invoices)
      {
        invoice.BusinessEntity = await _businessEntityService.GetAsync(invoice.BusinessEntityId, GetOrganizationId());
        invoice.InvoiceLines = await _generalLedgerInvoiceInvoiceLineService.GetByInvoiceIdAsync(invoice.InvoiceID, GetOrganizationId(), true);
      }
      return invoices;
    }

    private ChartOfAccountViewModel BuildChartOfAccountViewModel(ChartOfAccount account)
    {
      return new ChartOfAccountViewModel
      {
        ChartOfAccountID = account.ChartOfAccountID,
        Name = account.Name
      };
    }

    private ReceivePaymentForInvoiceViewModel BuildReceivePaymentForInvoiceViewModel(Invoice invoice)
    {
      var totalAmount = invoice.InvoiceLines!.Sum(il => il.Quantity * il.Price);
      return new ReceivePaymentForInvoiceViewModel
      {
        InvoiceId = invoice.InvoiceID,
        InvoiceNumber = invoice.InvoiceNumber!.ToString(),
        InvoiceLines = invoice.InvoiceLines!.Select(il => new ReceivePaymentForInvoiceViewModel.InvoiceLineViewModel
        {
          InvoiceLineID = il.InvoiceLineID,
          Title = il.Title,
          Description = il.Description,
          Quantity = il.Quantity,
          Price = il.Price,
          //AmountToReceive = il.Quantity * il.Price,
          RevenueChartOfAccountId = il.RevenueChartOfAccountId,
          AssetsChartOfAccountId = il.AssetsChartOfAccountId
        }).ToList(),
        BusinessEntity = new BusinessEntityViewModel
        {
          CompanyName = invoice.BusinessEntity!.CompanyName,
          FirstName = invoice.BusinessEntity!.FirstName,
          LastName = invoice.BusinessEntity!.LastName,
          CustomerType = invoice.BusinessEntity!.CustomerType
        },
        Total = totalAmount,
        Received = invoice.Received
      };
    }

    private ReceivePaymentForInvoiceIdsViewModel CreateReceivePaymentForInvoiceIdsViewModel(
      List<Invoice> invoices, 
      List<ChartOfAccount> debitAccounts)
    {
      var invoicesViewModel = invoices.Select(invoice => BuildReceivePaymentForInvoiceViewModel(invoice)).ToList();
      var accountsViewModel = debitAccounts.Select(BuildChartOfAccountViewModel).ToList();
      return new ReceivePaymentForInvoiceIdsViewModel
      {
        Invoices = invoicesViewModel,
        DebitAccounts = accountsViewModel
      };
    }
  }
}