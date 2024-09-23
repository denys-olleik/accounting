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

      List<JournalInvoiceInvoiceLinePayment> lastTransactions
        = await factoryManager.GetJournalInvoiceInvoiceLinePaymentManager()
        .GetLastTransactionsAsync(payment.PaymentID, organizationId, true);

      Guid transactionGuid = GuidExtensions.CreateSecureGuid();

      foreach (JournalInvoiceInvoiceLinePayment entry in lastTransactions)
      {
        Journal undoEntry = await factoryManager.GetJournalManager().CreateAsync(new Journal()
        {
          AccountId = entry.Journal!.AccountId,
          Credit = entry.Journal.Debit,
          Debit = entry.Journal.Credit,
          CreatedById = userId,
          OrganizationId = organizationId,
        });

        await factoryManager.GetJournalInvoiceInvoiceLinePaymentManager().CreateAsync(new JournalInvoiceInvoiceLinePayment()
        {
          JournalId = undoEntry.JournalID,
          ReversedJournalInvoiceInvoiceLinePaymentId = entry.JournalInvoiceInvoiceLinePaymentID,
          InvoiceInvoiceLinePaymentId = entry.InvoiceInvoiceLinePaymentId,
          TransactionGuid = transactionGuid,
          CreatedById = userId,
          OrganizationId = organizationId,
        });
      }
    }
  }
}