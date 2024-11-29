using Accounting.Business;
using Accounting.Models.Account;
using Accounting.Models.AddressViewModels;
using Accounting.Models.BusinessEntityViewModels;
using Accounting.Models.Item;
using Accounting.Models.PaymentTermViewModels;
using Accounting.Validators;
using FluentValidation;
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

    public class CreateInvoiceViewModelValidator : InvoiceViewModelValidatorBase<CreateInvoiceViewModel>
    {
      public CreateInvoiceViewModelValidator()
      {
        RuleFor(x => x.SelectedCustomerId)
          .NotNull()
          .WithMessage("Select a customer.")
          .DependentRules(() =>
          {
            RuleFor(x => x.SelectedBillingAddress).NotNull()
              .WithMessage("Select billing address.");
          });

        RuleFor(x => x.InvoiceLines)
          .NotEmpty()
          .WithMessage("'Invoice lines' cannot be empty.")
          .DependentRules(() =>
          {
            RuleFor(x => x.InvoiceLines)
                .MustAsync(async (invoiceLines, cancellationToken) => await BeValidInvoiceLineListAsync(invoiceLines))
                .WithMessage("One or more invoice lines are invalid.");

            RuleForEach(x => x.InvoiceLines)
                .ChildRules(invoiceLine =>
                {
                  invoiceLine.RuleFor(line => line.Quantity)
                      .GreaterThan(0)
                      .WithMessage("'Quantity' must be greater than 0 for all invoice lines.");
                });
          });

        RuleFor(x => x.DueDate)
            .NotNull()
            .WithMessage("'Due date' is required. Select payment terms.");
      }
    }
  }

  public class InvoiceLineViewModel
  {
    public int ID { get; set; }

    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool TitleOrDescriptionModified { get; set; }

    public decimal? Quantity { get; set; }
    public decimal? Price { get; set; }
    public bool QuantityOrPriceModified { get; set; }

    public int? RevenueAccountId { get; set; }
    public int? AssetsAccountId { get; set; }
  }
}