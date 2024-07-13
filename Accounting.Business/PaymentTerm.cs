using Accounting.Common;

namespace Accounting.Business
{
    public class PaymentTerm : IIdentifiable<int>
    {
        public int PaymentTermID { get; set; }
        public string? Description { get; set; }
        public int DaysUntilDue { get; set; }
        public int CreatedById { get; set; }
        public int OrganizationId { get; set; }

        public int Identifiable => this.PaymentTermID;
    }
}