using Accounting.Common;

namespace Accounting.Business
{
  public class InvoiceInvoiceLinePayment : IIdentifiable<int>
  {
    public int InvoiceInvoiceLinePaymentID { get; set; }
    public int InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
    public int InvoiceLineId { get; set; }
    public InvoiceLine? InvoiceLine { get; set; }
    public int PaymentId { get; set; }
    public Payment? Payment { get; set; }
    public decimal Amount { get; set; }
    public string VoidReason { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }
    public DateTime Created { get; set; }

    public int Identifiable => this.InvoiceInvoiceLinePaymentID;
    public int? RowNumber { get; set; }
  }
}