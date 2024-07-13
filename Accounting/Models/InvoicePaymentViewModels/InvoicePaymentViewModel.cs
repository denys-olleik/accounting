using Accounting.Business;
using Accounting.Models.InvoiceViewModels;
using Accounting.Models.PaymentViewModels;

namespace Accounting.Models.InvoicePaymentViewModels
{
    public class InvoicePaymentViewModel
    {
        public int ID { get; set; }
        public InvoiceViewModel? Invoice { get; set; }
        public PaymentViewModel? Payment { get; set; }
        public decimal Amount { get; set; }
        public DateTime Created { get; set; }

        public int? RowNumber { get; set; }
    }
}