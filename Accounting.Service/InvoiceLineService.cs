using Accounting.Business;
using Accounting.Database;

namespace Accounting.Service
{
  public class InvoiceLineService
  {
    private readonly string _databaseName;

    public InvoiceLineService(string databaseName)
    {
      _databaseName = databaseName;
    }

    public async Task<InvoiceLine> CreateAsync(InvoiceLine invoiceLine)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceLineManager().CreateAsync(invoiceLine);
    }

    public async Task<int> DeleteByInvoiceIdAsync(int invoiceId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceLineManager().DeleteByInvoiceIdAsync(invoiceId, organizationId);
    }

    public async Task<List<InvoiceLine>> GetByInvoiceIdAsync(int invoiceId, int organizationId)
    {
      throw new NotImplementedException(
        "Do not implement this method. Use journalInvoiceInvoiceLineService.GetByInvoiceIdAsync.");
    }

    public async Task<int> UpdateAsync(InvoiceLine line, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceLineManager().UpdateAsync(line, organizationId);
    }

    public async Task<int> UpdateTitleAndDescription(List<InvoiceLine> invoiceLines, int invoiceID, int userId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceLineManager().UpdateTitleAndDescription(invoiceLines, invoiceID, userId, organizationId);
    }
  }
}