namespace Accounting.Models.InvoiceViewModels
{
  public class GetInvoicesViewModel
  {
    public List<InvoiceViewModel>? Invoices { get; set; }
    public int? CurrentPage { get; set; }
    public int? NextPage { get; set; }
  }
}