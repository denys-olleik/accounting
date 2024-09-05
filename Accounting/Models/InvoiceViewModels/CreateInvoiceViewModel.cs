using Accounting.Business;
using Accounting.Models.Account;
using Accounting.Models.AddressViewModels;
using Accounting.Models.BusinessEntityViewModels;
using Accounting.Models.ItemViewModels;
using Accounting.Models.PaymentTermViewModels;
using FluentValidation.Results;

namespace Accounting.Models.InvoiceViewModels
{
  public class CreateInvoiceViewModel
  {
    public List<BusinessEntityViewModel>? Customers { get; set; }
    public BusinessEntityViewModel? SelectedCustomer { get; set; }
    public string? InvoiceNumber { get; set; }
    public int? SelectedCustomerId { get; set; }
    public AddressViewModel? SelectedBillingAddress { get; set; }
    public int? SelectedBillingAddressId { get; set; }
    public AddressViewModel? SelectedShippingAddress { get; set; }
    public int? SelectedShippingAddressId { get; set; }
    public List<string>? InvoiceStatuses { get; set; }
    public string? InvoiceLinesJson { get; set; }
    public List<ItemViewModel>? ProductsAndServices { get; set; }
    public List<InvoiceLineViewModel>? InvoiceLines { get; set; }
    public List<PaymentTermViewModel>? PaymentTerms { get; set; }
    public PaymentTermViewModel? SelectedPaymentTerm { get; set; }
    public string? SelectedPaymentTermJSON { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    public List<AccountViewModel>? DebitAccounts { get; set; }
    public int? SelectedDebitAccountId { get; set; }
    public List<AccountViewModel>? CreditAccounts { get; set; }
    public int? SelectedCreditAccountId { get; set; }
    public List<InvoiceAttachment>? InvoiceAttachments { get; set; }
    public string? InvoiceAttachmentsJSON { get; set; }
    public string? PaymentInstructions { get; set; }
    public bool RememberPaymentInstructions { get; set; }

    public ValidationResult? ValidationResult { get; set; }
    public int OrganizationId { get; set; }
  }
}