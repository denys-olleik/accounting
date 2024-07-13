using Accounting.Business;
using Accounting.Common;
using Accounting.Database;
using Accounting.Database.Interfaces;

namespace Accounting.Service
{
  public class InvoiceService
  {
    private readonly GeneralLedgerService _generalLedgerService;
    private readonly GeneralLedgerInvoiceInvoiceLineService _generalLedgerInvoiceInvoiceLineService;

    public InvoiceService(GeneralLedgerService generalLedgerService, GeneralLedgerInvoiceInvoiceLineService generalLedgerInvoiceInvoiceLineService)
    {
      _generalLedgerService = generalLedgerService;
      _generalLedgerInvoiceInvoiceLineService = generalLedgerInvoiceInvoiceLineService;
    }

    public async Task<Invoice> CreateAsync(Invoice invoice)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetInvoiceManager().CreateAsync(invoice);
    }

    public Task<bool> ExistsAsync(int invoiceId, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return factoryManager.GetInvoiceManager().ExistsAsync(invoiceId, organizationId);
    }

    public async Task<bool> IsVoidAsync(int invoiceId, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetInvoiceManager().IsVoidAsync(invoiceId, organizationId);
    }

    public async Task<(List<Invoice> Invoices, int? NextPageNumber)> GetAllAsync(
      int page,
      int pageSize,
      string[] paymentStatuses,
      int organizationId,
      bool includeVoidInvoices)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetInvoiceManager()
        .GetAllAsync(
          page,
          pageSize,
          paymentStatuses,
          organizationId,
          includeVoidInvoices);
    }

    public async Task<Invoice> GetAsync(int id, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetInvoiceManager().GetAsync(id, organizationId);
    }

    public async Task<List<Invoice>> GetAsync(string invoiceIdsCsv, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetInvoiceManager().GetAsync(invoiceIdsCsv, organizationId);
    }

    public async Task<int> ComputeAndUpdateInvoiceStatus(int invoiceId, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();

      string inPaymentStatus = await factoryManager.GetInvoiceManager().CalculateInvoiceStatusAsync(invoiceId, organizationId);

      return await factoryManager.GetInvoiceManager().UpdateStatusAsync(invoiceId, inPaymentStatus);
    }

    public async Task<List<Invoice>> SearchInvoicesAsync(string[] inPaymentStatus, string invoiceNumbersSpaceSeparated, string company, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();

      return await factoryManager.GetInvoiceManager().SearchInvoicesAsync(inPaymentStatus, invoiceNumbersSpaceSeparated, company, organizationId);
    }

    public async Task VoidAsync(
      Invoice invoice, 
      string? voidReason, 
      int userId, 
      int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      IGeneralLedgerInvoiceInvoiceLineManager generalLedgerInvoiceInvoiceLineManager = factoryManager.GetGeneralLedgerInvoiceInvoiceLineManager();
      await factoryManager.GetInvoiceManager().VoidAsync(invoice.InvoiceID, voidReason, organizationId);

      List<InvoiceLine> currentInvoiceLines
        = await _generalLedgerInvoiceInvoiceLineService
        .GetByInvoiceIdAsync(invoice.InvoiceID, organizationId, true);

      Guid transactionGuid = GuidExtensions.CreateSecureGuid();

      foreach (var lineItem in currentInvoiceLines)
      {
        List<GeneralLedgerInvoiceInvoiceLine> lastTransaction
          = await generalLedgerInvoiceInvoiceLineManager.GetLastTransactionAsync(
            lineItem.InvoiceLineID,
            organizationId,
            true);

        foreach (var gliil in lastTransaction)
        {
          GeneralLedger undoEntry = await _generalLedgerService.CreateAsync(new GeneralLedger()
          {
            ChartOfAccountId = gliil.GeneralLedger!.ChartOfAccountId,
            Credit = gliil.GeneralLedger.Debit,
            Debit = gliil.GeneralLedger.Credit,
            CreatedById = userId,
            OrganizationId = organizationId,
          });

          await generalLedgerInvoiceInvoiceLineManager.CreateAsync(new GeneralLedgerInvoiceInvoiceLine()
          {
            GeneralLedgerId = undoEntry.GeneralLedgerID,
            InvoiceLineId = lineItem.InvoiceLineID,
            InvoiceId = invoice.InvoiceID,
            ReversedGeneralLedgerInvoiceInvoiceLineId = gliil.GeneralLedgerInvoiceInvoiceLineID,
            TransactionGuid = transactionGuid,
            CreatedById = userId,
            OrganizationId = organizationId,
          });
        }
      }
    }

    public async Task<(decimal unpaid, decimal paid)> GetUnpaidAndPaidBalance(int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetInvoiceManager().GetUnpaidAndPaidBalance(organizationId);
    }

    public async Task<int> ComputeAndUpdateTotalAmountAndReceivedAmount(int invoiceId, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetInvoiceManager().ComputeAndUpdateTotalAmountAndReceivedAmount(invoiceId, organizationId);
    }

    public async Task UpdateLastUpdated(int invoiceId, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      await factoryManager.GetInvoiceManager().UpdateLastUpdated(invoiceId, organizationId);
    }

    public async Task<DateTime> GetLastUpdatedAsync(int invoiceId, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetInvoiceManager().GetLastUpdatedAsync(invoiceId, organizationId);
    }

    public async Task<List<Invoice>> GetAllAsync(int organizationId, string[] inPaymentStatus)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetInvoiceManager().GetAllAsync(organizationId, inPaymentStatus);
    }

    public async Task<int> UpdatePaymentInstructions(int invoiceId, string? paymentInstructions, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetInvoiceManager().UpdatePaymentInstructions(invoiceId, paymentInstructions, organizationId);
    }
  }
}