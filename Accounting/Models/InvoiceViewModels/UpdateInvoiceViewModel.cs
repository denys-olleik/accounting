using Accounting.Models.AddressViewModels;
using Accounting.Models.BusinessEntityViewModels;
using Accounting.Models.ItemViewModels;
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
    public AddressViewModel? SelectedAddress { get; set; }
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

    public ValidationResult? ValidationResult { get; set; }
  }
}