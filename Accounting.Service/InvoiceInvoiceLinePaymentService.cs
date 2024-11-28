using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class InvoiceInvoiceLinePaymentService
  {
    private readonly string _databaseName;

    public InvoiceInvoiceLinePaymentService(string databaseName)
    {
      _databaseName = databaseName;
    }

    public async Task<InvoiceInvoiceLinePayment> CreateAsync(InvoiceInvoiceLinePayment invoicePayment)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceInvoiceLinePaymentManager().CreateAsync(invoicePayment);
    }

    public async Task<(List<InvoiceInvoiceLinePayment> InvoicePayment, int? NextPageNumber)> GetAllAsync(
        int page, int pageSize, int organizationId, List<string> typesToLoad)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceInvoiceLinePaymentManager().GetAllAsync(page, pageSize, organizationId, typesToLoad);
    }

    public async Task<List<Invoice>> GetAllInvoicesByPaymentIdAsync(int paymentID, int organizationId, bool includeVoid = false)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceInvoiceLinePaymentManager().GetAllInvoicesByPaymentIdAsync(paymentID, organizationId);
    }

    public async Task<List<Payment>> GetAllPaymentsByInvoiceIdAsync(int invoiceId, int organizationId, bool includeVoid = false)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceInvoiceLinePaymentManager().GetAllPaymentsByInvoiceIdAsync(invoiceId, organizationId, includeVoid);
    }

    public async Task<InvoiceInvoiceLinePayment> GetInvoicePaymentAsync(int id, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceInvoiceLinePaymentManager().GetInvoicePaymentAsync(id, organizationId);
    }

    public async Task<decimal> GetTotalReceivedAsync(int invoiceLineId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceInvoiceLinePaymentManager().GetTotalReceivedAsync(invoiceLineId, organizationId);
    }

    public async Task<List<InvoiceInvoiceLinePayment>> GetValidInvoiceInvoiceLinePaymentsAsync(int invoiceId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceInvoiceLinePaymentManager().GetValidInvoicePaymentsAsync(invoiceId, organizationId);
    }

    public async Task<(List<InvoiceInvoiceLinePayment> InvoicePayments, int? NextPageNumber)> SearchInvoicePaymentsAsync(
      int page, int pageSize, string customerSearchQuery, List<string> typesToLoad, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceInvoiceLinePaymentManager().SearchInvoicePaymentsAsync(page, pageSize, customerSearchQuery, typesToLoad, organizationId);
    }

    public async Task<int> VoidAsync(int id, string voidReason, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceInvoiceLinePaymentManager().VoidAsync(id, voidReason, organizationId);
    }
  }
}