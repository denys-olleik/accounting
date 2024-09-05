using Accounting.Business;
using Accounting.Common;
using Accounting.Database;

namespace Accounting.Service
{
  public class PaymentService
  {
    public async Task<Payment> CreateAsync(Payment payment)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetPaymentManager().CreateAsync(payment);
    }

    public async Task<List<Payment>> GetAllByInvoiceIdAsync(int invoiceId, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetPaymentManager().GetAllByInvoiceIdAsync(invoiceId, organizationId);
    }

    public async Task<Payment> GetAsync(int id, int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      return await factoryManager.GetPaymentManager().GetAsync(id, organizationId);
    }

    public async Task VoidAsync(
      Payment payment,
      string? voidReason,
      int userId,
      int organizationId)
    {
      FactoryManager factoryManager = new FactoryManager();
      await factoryManager.GetPaymentManager().UpdateVoidReasonAsync(payment.PaymentID, voidReason, organizationId);

      List<GeneralLedgerInvoiceInvoiceLinePayment> lastTransactions
        = await factoryManager.GetGeneralLedgerInvoiceInvoiceLinePaymentManager()
        .GetLastTransactionsAsync(payment.PaymentID, organizationId, true);

      Guid transactionGuid = GuidExtensions.CreateSecureGuid();

      foreach (GeneralLedgerInvoiceInvoiceLinePayment entry in lastTransactions)
      {
        GeneralLedger undoEntry = await factoryManager.GetGeneralLedgerManager().CreateAsync(new GeneralLedger()
        {
          AccountId = entry.GeneralLedger!.AccountId,
          Credit = entry.GeneralLedger.Debit,
          Debit = entry.GeneralLedger.Credit,
          CreatedById = userId,
          OrganizationId = organizationId,
        });

        await factoryManager.GetGeneralLedgerInvoiceInvoiceLinePaymentManager().CreateAsync(new GeneralLedgerInvoiceInvoiceLinePayment()
        {
          GeneralLedgerId = undoEntry.GeneralLedgerID,
          ReversedGeneralLedgerInvoiceInvoiceLinePaymentId = entry.GeneralLedgerInvoiceInvoiceLinePaymentID,
          InvoiceInvoiceLinePaymentId = entry.InvoiceInvoiceLinePaymentId,
          TransactionGuid = transactionGuid,
          CreatedById = userId,
          OrganizationId = organizationId,
        });
      }
    }
  }
}