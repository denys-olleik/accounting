using Accounting.Models.AccountsReceivableViewModels.VoidInvoicePaymentViewModels;
using FluentValidation.Results;

namespace Accounting.Models.AccountsReceivableViewModels
{
    public class VoidInvoicePaymentViewModel
    {
        public int ID { get; set; }
        public InvoiceViewModel? Invoice { get; set; }
        public PaymentViewModel? Payment { get; set; }
        public BusinessEntityViewModel? BusinessEntity { get; set; }
        public decimal Amount { get; set; }

        public string? VoidReason { get; set; }

        public ValidationResult? ValidationResult { get; set; }
    }
}