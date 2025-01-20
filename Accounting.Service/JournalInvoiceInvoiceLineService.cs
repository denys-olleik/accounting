using Accounting.Business;
using Accounting.Common;
using Accounting.Database;

namespace Accounting.Service
{
  public class JournalInvoiceInvoiceLineService
  {
    private readonly string _databaseName;
    private readonly string _databasePassword;
    private readonly InvoiceLineService _invoiceLineService;
    private readonly JournalService _journalService;

    public JournalInvoiceInvoiceLineService(
      InvoiceLineService invoiceLineService,
      JournalService journalService,
      string databasePassword = "password",
      string databaseName = DatabaseThing.DatabaseConstants.Database)
    {
      _databaseName = databaseName;
      _databasePassword = databasePassword;
      _invoiceLineService = invoiceLineService;
      _journalService = journalService;
    }

    public async Task<JournalInvoiceInvoiceLine> CreateAsync(JournalInvoiceInvoiceLine journalInvoiceInvoiceLine)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetJournalInvoiceInvoiceLineManager().CreateAsync(journalInvoiceInvoiceLine);
    }

    public async Task UpdateInvoiceLinesAsync(
      List<InvoiceLine> existingLines,
      List<InvoiceLine> newLines,
      List<InvoiceLine> deletedLines,
      Invoice invoice,
      int userId,
      int organizationId)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      var journalInvoiceInvoiceLineManager = factoryManager.GetJournalInvoiceInvoiceLineManager();
      var transactionGuid = GuidExtensions.CreateSecureGuid();

      foreach (var invoiceLine in deletedLines)
      {
        var lastTransaction = await journalInvoiceInvoiceLineManager.GetLastTransactionAsync(invoiceLine.InvoiceLineID, organizationId, true);

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
            InvoiceLineId = invoiceLine.InvoiceLineID,
            InvoiceId = invoice.InvoiceID,
            ReversedJournalInvoiceInvoiceLineId = gliil.JournalInvoiceInvoiceLineID,
            TransactionGuid = transactionGuid,
            CreatedById = userId,
            OrganizationId = organizationId,
          });
        }
      }

      foreach (var line in existingLines)
      {
        var lastTransaction = await journalInvoiceInvoiceLineManager.GetLastTransactionAsync(line.InvoiceLineID, organizationId, true);
        var totalAmount = line.Quantity * line.Price;

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
            InvoiceLineId = line.InvoiceLineID,
            InvoiceId = invoice.InvoiceID,
            ReversedJournalInvoiceInvoiceLineId = gliil.JournalInvoiceInvoiceLineID,
            TransactionGuid = transactionGuid,
            CreatedById = userId,
            OrganizationId = organizationId,
          });

          var newEntry = await _journalService.CreateAsync(new Journal
          {
            AccountId = gliil.Journal.AccountId,
            Debit = gliil.Journal.Debit.HasValue ? totalAmount : (decimal?)null,
            Credit = gliil.Journal.Credit.HasValue ? totalAmount : (decimal?)null,
            CreatedById = userId,
            OrganizationId = organizationId,
          });

          await journalInvoiceInvoiceLineManager.CreateAsync(new JournalInvoiceInvoiceLine
          {
            JournalId = newEntry.JournalID,
            InvoiceLineId = line.InvoiceLineID,
            InvoiceId = invoice.InvoiceID,
            TransactionGuid = transactionGuid,
            CreatedById = userId,
            OrganizationId = organizationId,
          });
        }

        await _invoiceLineService.UpdateAsync(line, organizationId);
      }

      if (newLines != null)
      {
        foreach (var invoiceLine in newLines)
        {
          var debitGlEntry = await _journalService.CreateAsync(new Journal
          {
            AccountId = invoiceLine.AssetsAccountId!.Value,
            Debit = invoiceLine.Price * invoiceLine.Quantity,
            Credit = null,
            CreatedById = userId,
            OrganizationId = organizationId,
          });

          var creditGlEntry = await _journalService.CreateAsync(new Journal
          {
            AccountId = invoiceLine.RevenueAccountId!.Value,
            Debit = null,
            Credit = invoiceLine.Price * invoiceLine.Quantity,
            CreatedById = userId,
            OrganizationId = organizationId,
          });

          await CreateAsync(new JournalInvoiceInvoiceLine
          {
            JournalId = creditGlEntry.JournalID,
            InvoiceId = invoice.InvoiceID,
            InvoiceLineId = invoiceLine.InvoiceLineID,
            TransactionGuid = transactionGuid,
            CreatedById = userId,
            OrganizationId = organizationId,
          });

          await CreateAsync(new JournalInvoiceInvoiceLine
          {
            JournalId = debitGlEntry.JournalID,
            InvoiceId = invoice.InvoiceID,
            InvoiceLineId = invoiceLine.InvoiceLineID,
            TransactionGuid = transactionGuid,
            CreatedById = userId,
            OrganizationId = organizationId,
          });
        }
      }
    }

    public async Task<List<InvoiceLine>> GetByInvoiceIdAsync(int invoiceID, int organizationId, bool onlyCurrent = false)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetJournalInvoiceInvoiceLineManager().GetByInvoiceIdAsync(invoiceID, organizationId, onlyCurrent);
    }

    public async Task<List<JournalInvoiceInvoiceLine>> GetAllAsync(int invoiceId, int organizationId, bool includeRemoved)
    {
      var factoryManager = new FactoryManager(_databasePassword, _databaseName);
      return await factoryManager.GetJournalInvoiceInvoiceLineManager().GetAllAsync(invoiceId, organizationId, includeRemoved);
    }
  }
}