using Accounting.Business;
using Accounting.Common;
using Accounting.Database;

namespace Accounting.Service
{
  public class InvoiceService
  {
    private readonly JournalService _journalService;
    private readonly JournalInvoiceInvoiceLineService _journalInvoiceInvoiceLineService;
    private readonly string _databaseName;

    public InvoiceService(
        JournalService journalService,
        JournalInvoiceInvoiceLineService journalInvoiceInvoiceLineService,
        string databaseName)
    {
      _journalService = journalService;
      _journalInvoiceInvoiceLineService = journalInvoiceInvoiceLineService;
      _databaseName = databaseName;
    }

    public async Task<Invoice> CreateAsync(Invoice invoice)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceManager().CreateAsync(invoice);
    }

    public Task<bool> ExistsAsync(int invoiceId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return factoryManager.GetInvoiceManager().ExistsAsync(invoiceId, organizationId);
    }

    public async Task<bool> IsVoidAsync(int invoiceId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceManager().IsVoidAsync(invoiceId, organizationId);
    }

    public async Task<(List<Invoice> Invoices, int? NextPageNumber)> GetAllAsync(
        int page,
        int pageSize,
        string[] paymentStatuses,
        int organizationId,
        bool includeVoidInvoices)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceManager()
          .GetAllAsync(page, pageSize, paymentStatuses, organizationId, includeVoidInvoices);
    }

    public async Task<Invoice> GetAsync(int id, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceManager().GetAsync(id, organizationId);
    }

    public async Task<List<Invoice>> GetAsync(string invoiceIdsCsv, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceManager().GetAsync(invoiceIdsCsv, organizationId);
    }

    public async Task<int> ComputeAndUpdateInvoiceStatus(int invoiceId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      string inPaymentStatus = await factoryManager.GetInvoiceManager().CalculateInvoiceStatusAsync(invoiceId, organizationId);
      return await factoryManager.GetInvoiceManager().UpdateStatusAsync(invoiceId, inPaymentStatus);
    }

    public async Task<List<Invoice>> SearchInvoicesAsync(string[] inPaymentStatus, string invoiceNumbersSpaceSeparated, string company, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceManager().SearchInvoicesAsync(inPaymentStatus, invoiceNumbersSpaceSeparated, company, organizationId);
    }

    public async Task VoidAsync(Invoice invoice, string? voidReason, int userId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      var journalInvoiceInvoiceLineManager = factoryManager.GetJournalInvoiceInvoiceLineManager();
      await factoryManager.GetInvoiceManager().VoidAsync(invoice.InvoiceID, voidReason, organizationId);

      var currentInvoiceLines = await _journalInvoiceInvoiceLineService
          .GetByInvoiceIdAsync(invoice.InvoiceID, organizationId, true);

      var transactionGuid = GuidExtensions.CreateSecureGuid();

      foreach (var lineItem in currentInvoiceLines)
      {
        var lastTransaction = await journalInvoiceInvoiceLineManager.GetLastTransactionAsync(
            lineItem.InvoiceLineID,
            organizationId,
            true);

        foreach (var gliil in lastTransaction)
        {
          var undoEntry = await _journalService.CreateAsync(new Journal
          {
            AccountId = gliil.Journal!.AccountId,
            Credit = gliil.Journal.Debit,
            Debit = gliil.Journal.Credit,
            CreatedById = userId,
            OrganizationId = organizationId,
          });

          await journalInvoiceInvoiceLineManager.CreateAsync(new JournalInvoiceInvoiceLine
          {
            JournalId = undoEntry.JournalID,
            InvoiceLineId = lineItem.InvoiceLineID,
            InvoiceId = invoice.InvoiceID,
            ReversedJournalInvoiceInvoiceLineId = gliil.JournalInvoiceInvoiceLineID,
            TransactionGuid = transactionGuid,
            CreatedById = userId,
            OrganizationId = organizationId,
          });
        }
      }
    }

    public async Task<(decimal unpaid, decimal paid)> GetUnpaidAndPaidBalance(int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceManager().GetUnpaidAndPaidBalance(organizationId);
    }

    public async Task<int> ComputeAndUpdateTotalAmountAndReceivedAmount(int invoiceId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceManager().ComputeAndUpdateTotalAmountAndReceivedAmount(invoiceId, organizationId);
    }

    public async Task UpdateLastUpdated(int invoiceId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      await factoryManager.GetInvoiceManager().UpdateLastUpdated(invoiceId, organizationId);
    }

    public async Task<DateTime> GetLastUpdatedAsync(int invoiceId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceManager().GetLastUpdatedAsync(invoiceId, organizationId);
    }

    public async Task<List<Invoice>> GetAllAsync(int organizationId, string[] inPaymentStatus)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceManager().GetAllAsync(organizationId, inPaymentStatus);
    }

    public async Task<int> UpdatePaymentInstructions(int invoiceId, string? paymentInstructions, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceManager().UpdatePaymentInstructions(invoiceId, paymentInstructions, organizationId);
    }
  }
}