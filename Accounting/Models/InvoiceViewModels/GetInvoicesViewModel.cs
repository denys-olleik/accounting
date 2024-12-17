namespace Accounting.Models.InvoiceViewModels
{
  public class GetInvoicesViewModel : PaginatedViewModel
  {
    public List<InvoiceViewModel>? Invoices { get; set; }
  }
}