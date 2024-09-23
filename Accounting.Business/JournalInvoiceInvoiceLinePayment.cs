using Accounting.Common;

namespace Accounting.Business
{
  public class JournalInvoiceInvoiceLinePayment : JournalTableBase, IIdentifiable<int>
  {
    public int JournalInvoiceInvoiceLinePaymentID { get; set; }
    public int InvoiceInvoiceLinePaymentId { get; set; }
    public int? ReversedJournalInvoiceInvoiceLinePaymentId { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }

    public int Identifiable => this.JournalInvoiceInvoiceLinePaymentID;
  }
}