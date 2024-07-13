using FluentValidation.Results;

namespace Accounting.Models.PaymentTermViewModels
{
    public class CreatePaymentTermsViewModel
    {
        public string? Description { get; set; }
        public int DaysUntilDue { get; set; }

        public ValidationResult? ValidationResult { get; set; }
    }
}