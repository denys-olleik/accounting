using Accounting.Common;

namespace Accounting.Business
{
  public class JournalInvoice : JournalTableBase, IIdentifiable<int>
  {
    // YO!!! Next time you're in this class, note that it inherits from JournalTableBase. Dumbass!
    // There you will find what you're probably looking for. lol
    // It's been 4 times I've done this now.
    public int JournalInvoiceID { get; set; }
    public int InvoiceId { get; set; }
    public int? ReversedJournalInvoiceId { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }

    public int Identifiable => this.JournalInvoiceID;
  }
}