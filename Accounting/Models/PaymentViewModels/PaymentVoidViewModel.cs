using FluentValidation.Results;

namespace Accounting.Models.PaymentViewModels
{
  public class PaymentVoidViewModel
  {
    public int PaymentID { get; set; }
    public string? VoidReason { get; set; }
    public string? ReferenceNumber { get; set; }
    public decimal? Amount { get; set; }

    public ValidationResult? ValidationResult { get; set; }
  }
}