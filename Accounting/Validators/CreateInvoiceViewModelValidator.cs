﻿using Accounting.Business;
using Accounting.Database;
using Accounting.Models.InvoiceViewModels;
using FluentValidation;

namespace Accounting.Validators
{
  public class CreateInvoiceViewModelValidator : InvoiceViewModelValidatorBase<CreateInvoiceViewModel>
  {
    private readonly int _organizationId;

    public CreateInvoiceViewModelValidator(int organizationId)
    {
      _organizationId = organizationId;

      RuleFor(x => x.SelectedCustomerId)
          .NotNull()
          .WithMessage("Please select a customer.")
          .DependentRules(() =>
          {
            RuleFor(x => x.SelectedCustomerId)
                      .MustAsync(CustomerExists)
                      .WithMessage("Selected customer does not exist.");

            RuleFor(x => x.SelectedBillingAddress).NotNull()
                      .WithMessage("Please select billing address.");
          });

      RuleFor(x => x.InvoiceLines)
        .NotEmpty()
        .WithMessage("Invoice Lines cannot be empty.")
        .DependentRules(() =>
          {
            RuleFor(x => x.InvoiceLines)
                .MustAsync(async (invoiceLines, cancellationToken) => await BeValidInvoiceLineListAsync(invoiceLines))
                .WithMessage("One or more invoice lines are invalid.");
          });

      RuleFor(x => x.SelectedPaymentTerm)
          .NotNull()
          .WithMessage("Payment Term is required.");

      RuleFor(x => x.DueDate)
          .NotNull()
          .WithMessage("Due Date is required.");
    }

    private async Task<bool> CustomerExists(int? entityId, CancellationToken token)
    {
      if (entityId == null)
        return false;

      FactoryManager factoryManager = new FactoryManager();
      BusinessEntity customer = await factoryManager.GetBusinessEntityManager().GetByIdAsync(entityId.Value, _organizationId);
      return customer != null;
    }
  }
}