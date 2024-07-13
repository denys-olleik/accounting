using Accounting.Models.InvoiceViewModels;
using Accounting.Service;
using FluentValidation;
using Accounting.Common;

namespace Accounting.Validators
{
  public class UpdateInvoiceViewModelValidator : InvoiceViewModelValidatorBase<UpdateInvoiceViewModel>
  {
    private readonly InvoiceService _invoiceService;

    public UpdateInvoiceViewModelValidator(InvoiceService invoiceService, int organizationId)
    {
      _invoiceService = invoiceService;

      RuleFor(x => x.ID)
          .MustAsync(async (id, cancellation) => !await _invoiceService.IsVoidAsync(id, organizationId))
          .WithMessage(x => $"Invoice '{x.InvoiceNumber}' is already void.");

      RuleFor(x => x.ExistingInvoiceLines)
        .MustAsync(async (existingInvoiceLines, cancellationToken) =>
            existingInvoiceLines == null || !existingInvoiceLines.Any() || await BeValidInvoiceLineListAsync(existingInvoiceLines))
        .WithMessage("One or more existing invoice lines are invalid.");

      RuleFor(x => x.NewInvoiceLines)
        .MustAsync(async (newInvoiceLines, cancellationToken) =>
            newInvoiceLines == null || !newInvoiceLines.Any() || await BeValidInvoiceLineListAsync(newInvoiceLines))
        .WithMessage("One or more new invoice lines are invalid.");

      RuleFor(x => x.LastUpdated)
        .MustAsync(async (model, lastUpdated, cancellation) =>
        {
          var currentLastUpdated = await _invoiceService.GetLastUpdatedAsync(model.ID, organizationId);
          return currentLastUpdated.RoundToSeconds() == lastUpdated.RoundToSeconds();
        })
        .WithMessage("The invoice has been updated since you last loaded it. Please refresh and try again.");
    }
  }
}