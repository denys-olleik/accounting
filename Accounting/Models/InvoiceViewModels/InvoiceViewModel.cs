using Accounting.Models.BusinessEntityViewModels;

namespace Accounting.Models.InvoiceViewModels
{
  public class InvoiceViewModel
  {
    public int InvoiceID { get; set; }
    public string? InvoiceNumber { get; set; }
    public BusinessEntityViewModel? BusinessEntity { get; set; }
    public string? Status { get; set; }
    public List<InvoiceLineViewModel>? InvoiceLines { get; set; }
    public List<PaymentViewModel>? Payments { get; set; }

    public decimal? Total { get; set; }
    public decimal? Received { get; set; }

    public int? RowNumber { get; set; }

    public class PaymentViewModel
    {
      public int PaymentID { get; set; }
      public string? ReferenceNumber { get; set; }
      public decimal? Amount { get; set; }
      public string? VoidReason { get; set; }
    }
  }
}