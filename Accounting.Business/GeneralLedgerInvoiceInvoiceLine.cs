using Accounting.Common;

namespace Accounting.Business
{
  public class GeneralLedgerInvoiceInvoiceLine : IIdentifiable<int>
  {
    public int GeneralLedgerInvoiceInvoiceLineID { get; set; }
    public int GeneralLedgerId { get; set; }
    public GeneralLedger? GeneralLedger { get; set; }
    public int InvoiceId { get; set; }
    public int InvoiceLineId { get; set; }
    public int? ReversedGeneralLedgerInvoiceInvoiceLineId { get; set; }
    public Guid TransactionGuid { get; set; }
    public DateTime Created { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }

    public int Identifiable => this.GeneralLedgerInvoiceInvoiceLineID;
  }
}