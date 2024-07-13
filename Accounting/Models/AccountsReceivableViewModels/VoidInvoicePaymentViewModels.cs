namespace Accounting.Models.AccountsReceivableViewModels.VoidInvoicePaymentViewModels
{
    public class InvoiceViewModel
    {
        public int ID { get; set; }
        public string? InvoiceNumber { get; set; }
    }

    public class PaymentViewModel
    {
        public int ID { get; set; }
        public string? ReferenceNumber { get; set; }
    }

    public class BusinessEntityViewModel
    {
        public int ID { get; set; }
        public string? DisplayName { get; set; }
    }
}