using Accounting.Business;

namespace Accounting.Database.Interfaces
{
  public interface IInvoiceInvoiceLinePaymentManager : IGenericRepository<InvoiceInvoiceLinePayment, int>
  {
    Task<(List<InvoiceInvoiceLinePayment> InvoicePayments, int? NextPageNumber)> GetAllAsync(int page, int pageSize, int organiztionId, List<string> typesToLoad = null);
    Task<List<Invoice>> GetAllInvoicesByPaymentIdAsync(int paymentId, int organizationId);
    Task<List<Payment>> GetAllPaymentsByInvoiceIdAsync(int invoiceId, int organizationId, bool includeVoid = false);
    Task<InvoiceInvoiceLinePayment> GetInvoicePaymentAsync(int id, int organizationId);
    Task<decimal> GetTotalReceivedAsync(int invoiceId, int organizationId);
    Task<List<InvoiceInvoiceLinePayment>> GetValidInvoicePaymentsAsync(int invoiceId, int organizationId);
    Task<(List<InvoiceInvoiceLinePayment> InvoicePayments, int? NextPageNumber)> SearchInvoicePaymentsAsync(int page, int pageSize, string searchQuery, List<string> typesToLoad, int organizationId);
    Task<int> VoidAsync(int id, string voidReason, int organizationId);
  }
}