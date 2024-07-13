using Accounting.Business;
using Accounting.Models.BusinessEntityViewModels;

namespace Accounting.Models.AccountsReceivableViewModels
{
  public class ReceivePaymentForInvoiceViewModel
  {
    public int InvoiceId { get; set; } //TODO change to D uppercase
    public List<InvoiceLineViewModel>? InvoiceLines { get; set; }
    public string? InvoiceNumber { get; set; }
    public decimal? Received { get; set; }
    public decimal? Total { get; set; }
    public decimal? Amount { get; set; }
    public decimal? AmountToReceive { get; set; }

    public BusinessEntityViewModel? BusinessEntity { get; set; }

    public class InvoiceLineViewModel
    {
      public int InvoiceLineID { get; set; }
      public string? Title { get; set; }
      public string? Description { get; set; }
      public decimal? Quantity { get; set; }
      public decimal? Price { get; set; }
      public decimal? Received { get; set; }
      public decimal? AmountToReceive { get; set; }
      public int RevenueChartOfAccountId { get; set; }
      public int AssetsChartOfAccountId { get; set; }
    }
  }
}