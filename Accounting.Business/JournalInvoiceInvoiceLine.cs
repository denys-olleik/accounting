using Accounting.Common;

namespace Accounting.Business
{
  public class JournalInvoiceInvoiceLine : IIdentifiable<int>
  {
    public int JournalInvoiceInvoiceLineID { get; set; }
    public int JournalId { get; set; }
    public Journal? Journal { get; set; }
    public int InvoiceId { get; set; }
    public int InvoiceLineId { get; set; }
    public int? ReversedJournalInvoiceInvoiceLineId { get; set; }
    public Guid TransactionGuid { get; set; }
    public DateTime Created { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }

    public int Identifiable => this.JournalInvoiceInvoiceLineID;
  }
}