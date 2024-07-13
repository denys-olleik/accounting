using Accounting.Common;

namespace Accounting.Business
{
  public class GeneralLedgerInvoiceInvoiceLinePayment : GeneralLedgerTableBase, IIdentifiable<int>
  {
    public int GeneralLedgerInvoiceInvoiceLinePaymentID { get; set; }
    public int InvoiceInvoiceLinePaymentId { get; set; }
    public int? ReversedGeneralLedgerInvoiceInvoiceLinePaymentId { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }

    public int Identifiable => this.GeneralLedgerInvoiceInvoiceLinePaymentID;
  }
}