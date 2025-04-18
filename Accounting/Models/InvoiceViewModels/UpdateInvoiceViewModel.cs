using Accounting.Models.AddressViewModels;
using Accounting.Models.BusinessEntityViewModels;
using Accounting.Models.Item;
using Accounting.Models.PaymentTermViewModels;
using FluentValidation.Results;

namespace Accounting.Models.InvoiceViewModels
{
  public class UpdateInvoiceViewModel
  {
    public int ID { get; set; }
    public BusinessEntityViewModel? Customer { get; set; }
    public string? InvoiceNumber { get; set; }
    public int? SelectedCustomerId { get; set; }
    public int? SelectedAddressId { get; set; }
    public AddressViewModel? BillingAddress { get; set; }
    public AddressViewModel? ShippingAddress { get; set; }
    public List<string>? InvoiceStatuses { get; set; }
    public string? InvoiceLinesJson { get; set; }
    public string? DeletedInvoiceLinesJson { get; set; }
    public List<ItemViewModel>? ProductsAndServices { get; set; }
    public List<InvoiceLineViewModel>? ExistingInvoiceLines { get; set; }
    public List<InvoiceLineViewModel>? NewInvoiceLines { get; set; }
    public List<InvoiceLineViewModel>? DeletedInvoiceLines { get; set; }
    public List<PaymentTermViewModel>? PaymentTerms { get; set; }
    public PaymentTermViewModel? SelectedPaymentTerm { get; set; }
    public string? SelectedPaymentTermJSON { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime LastUpdated { get; set; }

    public string? DeletedAttachmentIdsCsv { get; set; }
    public string? NewAttachmentIdsCsv { get; set; }

    public List<InvoiceAttachmentViewModel> Attachments { get; set; } = new List<InvoiceAttachmentViewModel>();

    public ValidationResult? ValidationResult { get; set; }

    public class InvoiceAttachmentViewModel
    {
      public int InvoiceAttachmentID { get; set; }
      public string FileName { get; set; }
    }
  }
}