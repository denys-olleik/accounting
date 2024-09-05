using Accounting.Business;
using Accounting.Common;
using Accounting.Database;
using Accounting.Database.Interfaces;

namespace Accounting.Service
{
  public class GeneralLedgerInvoiceInvoiceLineService
  {
    private readonly InvoiceLineService _invoiceLineService;
    private readonly GeneralLedgerService _generalLedgerService;

    public GeneralLedgerInvoiceInvoiceLineService(InvoiceLineService invoiceLineService, GeneralLedgerService generalLedgerService)
    {
      _invoiceLineService = invoiceLineService;
      _generalLedgerService = generalLedgerService;
    }

    public async Task<GeneralLedgerInvoiceInvoiceLine> CreateAsync(GeneralLedgerInvoiceInvoiceLine generalLedger_Invoice_InvoiceLine)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetGeneralLedgerInvoiceInvoiceLineManager().CreateAsync(generalLedger_Invoice_InvoiceLine);
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
      IGeneralLedgerInvoiceInvoiceLineManager generalLedgerInvoiceInvoiceLineManager 
        = factoryManager.GetGeneralLedgerInvoiceInvoiceLineManager();

      Guid transactionGuid = GuidExtensions.CreateSecureGuid();

      foreach (InvoiceLine invoiceLine in deletedLines)
      {
        List<GeneralLedgerInvoiceInvoiceLine> lastTransaction
          = await generalLedgerInvoiceInvoiceLineManager.GetLastTransactionAsync(
            invoiceLine.InvoiceLineID, 
            organizationId, 
            true);

        foreach (var gliil in lastTransaction)
        {
          GeneralLedger undoEntry = await _generalLedgerService.CreateAsync(new GeneralLedger()
          {
            AccountId = gliil.GeneralLedger!.AccountId,
            Credit = gliil.GeneralLedger.Debit,
            Debit = gliil.GeneralLedger.Credit,
            CreatedById = userId,
            OrganizationId = organizationId,
          });

          await generalLedgerInvoiceInvoiceLineManager.CreateAsync(new GeneralLedgerInvoiceInvoiceLine()
          {
            GeneralLedgerId = undoEntry.GeneralLedgerID,
            InvoiceLineId = invoiceLine.InvoiceLineID,
            InvoiceId = invoice.InvoiceID,
            ReversedGeneralLedgerInvoiceInvoiceLineId = gliil.GeneralLedgerInvoiceInvoiceLineID,
            TransactionGuid = transactionGuid,
            CreatedById = userId,
            OrganizationId = organizationId,
          });
        }
      }

      foreach (InvoiceLine line in existingLines)
      {
        List<GeneralLedgerInvoiceInvoiceLine> lastTransaction
          = await generalLedgerInvoiceInvoiceLineManager.GetLastTransactionAsync(
            line.InvoiceLineID, 
            organizationId, 
            true);

        decimal? totalAmount = line.Quantity * line.Price;

        foreach (var gliil in lastTransaction)
        {
          GeneralLedger undoEntry
            = await _generalLedgerService.CreateAsync(new GeneralLedger()
          {
            AccountId = gliil.GeneralLedger!.AccountId,
            Credit = gliil.GeneralLedger.Debit,
            Debit = gliil.GeneralLedger.Credit,
            CreatedById = userId,
            OrganizationId = organizationId,
          });

          await generalLedgerInvoiceInvoiceLineManager.CreateAsync(new GeneralLedgerInvoiceInvoiceLine()
          {
            GeneralLedgerId = undoEntry.GeneralLedgerID,
            InvoiceLineId = line.InvoiceLineID,
            InvoiceId = invoice.InvoiceID,
            ReversedGeneralLedgerInvoiceInvoiceLineId = gliil.GeneralLedgerInvoiceInvoiceLineID,
            TransactionGuid = transactionGuid,
            CreatedById = userId,
            OrganizationId = organizationId,
          });

          GeneralLedger newEntry = await _generalLedgerService.CreateAsync(new GeneralLedger()
          {
            AccountId = gliil.GeneralLedger.AccountId,
            Debit = gliil.GeneralLedger.Debit.HasValue ? totalAmount : (decimal?)null,
            Credit = gliil.GeneralLedger.Credit.HasValue ? totalAmount : (decimal?)null,
            CreatedById = userId,
            OrganizationId = organizationId,
          });

          await generalLedgerInvoiceInvoiceLineManager.CreateAsync(new GeneralLedgerInvoiceInvoiceLine()
          {
            GeneralLedgerId = newEntry.GeneralLedgerID,
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
          GeneralLedger debitGlEntry = await _generalLedgerService.CreateAsync(new GeneralLedger()
          {
            AccountId = invoiceLine.AssetsAccountId,
            Debit = invoiceLine.Price * invoiceLine.Quantity,
            Credit = null,
            CreatedById = userId,
            OrganizationId = organizationId,
          });

          GeneralLedger creditGlEntry = await _generalLedgerService.CreateAsync(new GeneralLedger()
          {
            AccountId = invoiceLine.RevenueAccountId,
            Debit = null,
            Credit = invoiceLine.Price * invoiceLine.Quantity,
            CreatedById = userId,
            OrganizationId = organizationId,
          });

          await CreateAsync(new GeneralLedgerInvoiceInvoiceLine()
          {
            GeneralLedgerId = creditGlEntry.GeneralLedgerID,
            InvoiceId = invoice.InvoiceID,
            InvoiceLineId = invoiceLine.InvoiceLineID,
            TransactionGuid = transactionGuid,
            CreatedById = userId,
            OrganizationId = organizationId,
          });

          await CreateAsync(new GeneralLedgerInvoiceInvoiceLine()
          {
            GeneralLedgerId = debitGlEntry.GeneralLedgerID,
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
      return await factoryManager.GetGeneralLedgerInvoiceInvoiceLineManager().GetByInvoiceIdAsync(invoiceID, organizationId, onlyCurrent); 
    }

    public async Task<List<GeneralLedgerInvoiceInvoiceLine>> GetAllAsync(int invoiceId, int organizationId, bool includeRemoved)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetGeneralLedgerInvoiceInvoiceLineManager().GetAllAsync(invoiceId, organizationId, includeRemoved);
    }
  }
}