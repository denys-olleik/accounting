using Accounting.Common;

namespace Accounting.Business
{
  public class GeneralLedgerInvoice : GeneralLedgerTableBase, IIdentifiable<int>
  {
    // YO!!! Next time you're in this class, note that it inherits from GeneralLedgerTableBase. Dumbass!
    // There you will find what you're probably looking for. lol
    // It's been 4 times I've done this now.
    public int GeneralLedgerInvoiceID { get; set; }
    public int InvoiceId { get; set; }
    public int? ReversedGeneralLedgerInvoiceId { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }

    public int Identifiable => this.GeneralLedgerInvoiceID;
  }
}