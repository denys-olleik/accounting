using Accounting.Business;

namespace Accounting.Database.Interfaces
{
    public interface IPaymentManager : IGenericRepository<Payment, int>
    {
    Task<List<Payment>> GetAllByInvoiceIdAsync(int invoiceId, int organizationId);
    Task<Payment> GetAsync(int id, int organizationId);
    Task UpdateVoidReasonAsync(int paymentId, string? voidReason, int organizationId);
  }
}