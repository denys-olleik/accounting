using Accounting.Models.InvoiceViewModels;
using FluentValidation;

namespace Accounting.Validators
{
  public abstract class InvoiceViewModelValidatorBase<T> : AbstractValidator<T>
  {
    protected async Task<bool> BeValidInvoiceLineListAsync(List<InvoiceLineViewModel>? invoiceLines)
    {
      if (invoiceLines == null || !invoiceLines.Any())
      {
        return false;
      }

      var validator = new InvoiceLineViewModelValidator();

      foreach (var invoiceLine in invoiceLines)
      {
        var result = await validator.ValidateAsync(invoiceLine);
        if (!result.IsValid)
        {
          return false;
        }
      }

      return true;
    }
  }
}