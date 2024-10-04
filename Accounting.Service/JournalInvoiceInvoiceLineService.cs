using Accounting.Business;
using Accounting.Common;
using Accounting.Database;
using Accounting.Database.Interfaces;

namespace Accounting.Service
{
  public class JournalInvoiceInvoiceLineService
  {
    private readonly InvoiceLineService _invoiceLineService;
    private readonly JournalService _journalService;

    public JournalInvoiceInvoiceLineService(InvoiceLineService invoiceLineService, JournalService journalService)
    {
      _invoiceLineService = invoiceLineService;
      _journalService = journalService;
    }

    public async Task<JournalInvoiceInvoiceLine> CreateAsync(JournalInvoiceInvoiceLine journal_Invoice_InvoiceLine)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetJournalInvoiceInvoiceLineManager().CreateAsync(journal_Invoice_InvoiceLine);
    }

    public async Task UpdateInvoiceLinesAsync(
      List<InvoiceLine> existingLines, 
      List<InvoiceLine> newLines,
      List<InvoiceLine> deletedLines,
      Invoice invoice, 
      int userId,
      int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      IJournalInvoiceInvoiceLineManager journalInvoiceInvoiceLineManager 
        = factoryManager.GetJournalInvoiceInvoiceLineManager();

      Guid transactionGuid = GuidExtensions.CreateSecureGuid();

      foreach (InvoiceLine invoiceLine in deletedLines)
      {
        List<JournalInvoiceInvoiceLine> lastTransaction
          = await journalInvoiceInvoiceLineManager.GetLastTransactionAsync(
            invoiceLine.InvoiceLineID, 
            organizationId, 
            true);

        foreach (var gliil in lastTransaction)
        {
          Journal undoEntry = await _journalService.CreateAsync(new Journal()
          {
            AccountId = gliil.Journal!.AccountId,
            Credit = gliil.Journal.Debit,
            Debit = gliil.Journal.Credit,
            CreatedById = userId,
            OrganizationId = organizationId,
          });

          await journalInvoiceInvoiceLineManager.CreateAsync(new JournalInvoiceInvoiceLine()
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

      foreach (InvoiceLine line in existingLines)
      {
        List<JournalInvoiceInvoiceLine> lastTransaction
          = await journalInvoiceInvoiceLineManager.GetLastTransactionAsync(
            line.InvoiceLineID, 
            organizationId, 
            true);

        decimal? totalAmount = line.Quantity * line.Price;

        foreach (var gliil in lastTransaction)
        {
          Journal undoEntry
            = await _journalService.CreateAsync(new Journal()
          {
            AccountId = gliil.Journal!.AccountId,
            Credit = gliil.Journal.Debit,
            Debit = gliil.Journal.Credit,
            CreatedById = userId,
            OrganizationId = organizationId,
          });

          await journalInvoiceInvoiceLineManager.CreateAsync(new JournalInvoiceInvoiceLine()
          {
            JournalId = undoEntry.JournalID,
            InvoiceLineId = line.InvoiceLineID,
            InvoiceId = invoice.InvoiceID,
            ReversedJournalInvoiceInvoiceLineId = gliil.JournalInvoiceInvoiceLineID,
            TransactionGuid = transactionGuid,
            CreatedById = userId,
            OrganizationId = organizationId,
          });

          Journal newEntry = await _journalService.CreateAsync(new Journal()
          {
            AccountId = gliil.Journal.AccountId,
            Debit = gliil.Journal.Debit.HasValue ? totalAmount : (decimal?)null,
            Credit = gliil.Journal.Credit.HasValue ? totalAmount : (decimal?)null,
            CreatedById = userId,
            OrganizationId = organizationId,
          });

          await journalInvoiceInvoiceLineManager.CreateAsync(new JournalInvoiceInvoiceLine()
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
        foreach (var invoiceLine in newLines!)
        {
          Journal debitGlEntry = await _journalService.CreateAsync(new Journal()
          {
            AccountId = invoiceLine.AssetsAccountId!.Value,
            Debit = invoiceLine.Price * invoiceLine.Quantity,
            Credit = null,
            CreatedById = userId,
            OrganizationId = organizationId,
          });

          Journal creditGlEntry = await _journalService.CreateAsync(new Journal()
          {
            AccountId = invoiceLine.RevenueAccountId!.Value,
            Debit = null,
            Credit = invoiceLine.Price * invoiceLine.Quantity,
            CreatedById = userId,
            OrganizationId = organizationId,
          });

          await CreateAsync(new JournalInvoiceInvoiceLine()
          {
            JournalId = creditGlEntry.JournalID,
            InvoiceId = invoice.InvoiceID,
            InvoiceLineId = invoiceLine.InvoiceLineID,
            TransactionGuid = transactionGuid,
            CreatedById = userId,
            OrganizationId = organizationId,
          });

          await CreateAsync(new JournalInvoiceInvoiceLine()
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
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetJournalInvoiceInvoiceLineManager().GetByInvoiceIdAsync(invoiceID, organizationId, onlyCurrent); 
    }

    public async Task<List<JournalInvoiceInvoiceLine>> GetAllAsync(int invoiceId, int organizationId, bool includeRemoved)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetJournalInvoiceInvoiceLineManager().GetAllAsync(invoiceId, organizationId, includeRemoved);
    }
  }
}