using FluentValidation.Results;

namespace Accounting.Models.PaymentInstructionViewModels
{
    public class CreatePaymentInstructionViewModel
    {
        public string? Title { get; set; }
        public string? Content { get; set; }

        public ValidationResult? ValidationResult { get; set; }
    }
}