using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IInvoiceManager : IGenericRepository<Invoice, int>
  {
    Task<string> CalculateInvoiceStatusAsync(int invoiceId, int organizationId);
    Task<int> ComputeAndUpdateTotalAmountAndReceivedAmount(int invoiceId, int organizationId);
    Task<bool> ExistsAsync(int invoiceId, int organizationId);
    Task<(List<Invoice> invoices, int? nextPage)> GetAllAsync(int page, int pageSize, string[] paymentStatuses, int organizationId, bool includeVoidInvoices);
    Task<List<Invoice>> GetAllAsync(int organizationId, string[] inPaymentStatus);
    Task<Invoice> GetAsync(int id, int organizationId);
    Task<List<Invoice>> GetAsync(string invoiceIdsCsv, int organizationId);
    Task<List<Invoice>> SearchInvoicesAsync(string[] inPaymentStatus, string invoiceNumber, string company, int organizationId);
    Task<DateTime> GetLastUpdatedAsync(int invoiceId, int organizationId);
    Task<(decimal unpaid, decimal paid)> GetUnpaidAndPaidBalance(int organizationId);
    Task<bool> IsVoidAsync(int invoiceId, int organizationId);
    Task<int> UpdateLastUpdated(int invoiceId, int organizationId);
    Task<int> UpdateStatusAsync(int invoiceId, string paymentStatus);
    Task<int> VoidAsync(int invoiceId, string? voidReason, int organizationId);
    Task<int> UpdatePaymentInstructions(int invoiceId, string? paymentInstructions, int organizationId);
  }
}