namespace Accounting.Models.InvoiceViewModels
{
  public class ViewInvoiceViewModel
  {
    public string? InvoiceNumber { get; set; }
    public DateTime? DueDate { get; set; }
    public string? DisplayDueDate => DueDate?.ToString("MMM dd yyyy");
    public string? OrganizationName { get; set; }
    public string? CustomerName { get; set; }
    public string? Address { get; set; }
    public string? AddressHtml { get; set; }
    public string? AccountsReceivableEmail { get; set; }
    public string? AccountsPayableEmail { get; set; }
    public string? AccountsReceivablePhone { get; set; }
    public string? AccountsPayablePhone { get; set; }
    public string? Website { get; set; }

    public AddressViewModel? BillingAddress { get; set; }
    public AddressViewModel? ShippingAddress { get; set; }

    public List<InvoiceLineViewModel>? InvoiceLines { get; set; }
    public decimal? InvoiceTotal => InvoiceLines?.Sum(x => x.LineTotal);
    public string? DisplayInvoiceTotal => InvoiceTotal?.ToString("C");

    public string? PaymentInstructions { get; set; }

    public class AddressViewModel
    {
      public int AddressID { get; set; }
      public string? ExtraAboveAddress { get; set; }
      public string? AddressLine1 { get; set; }
      public string? AddressLine2 { get; set; }
      public string? ExtraBelowAddress { get; set; }
      public string? City { get; set; }
      public string? StateProvince { get; set; }
      public string? PostalCode { get; set; }
      public string? Country { get; set; }
    }

    public class InvoiceLineViewModel
    {
      public int InvoiceLineID { get; set; }
      public int RowNumber { get; set; }
      public string? Title { get; set; }
      public string? Description { get; set; }
      public decimal? Quantity { get; set; }
      public decimal? Price { get; set; }
      public decimal? LineTotal { get; set; }
    }
  }
}