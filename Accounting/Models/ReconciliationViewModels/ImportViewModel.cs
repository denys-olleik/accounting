using FluentValidation.Results;

namespace Accounting.Models.ReconciliationViewModels
{
  public class ImportViewModel
  {
    public ValidationResult? ValidationResult { get; set; }
  }
}