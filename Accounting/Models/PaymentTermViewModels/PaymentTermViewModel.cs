namespace Accounting.Models.PaymentTermViewModels
{
    public class PaymentTermViewModel
    {
        public int ID { get; set; }
        public string? Description { get; set; }
        public int DaysUntilDue { get; set; }

        public string DisplayText
        {
            get
            {
                return $"{Description} ({DaysUntilDue} days)";
            }
        }
    }
}