using Accounting.Common;

namespace Accounting.Business
{
    public class InvoiceAttachment : IIdentifiable<int>
    {
        public int InvoiceAttachmentID { get; set; }
        public int? InvoiceId { get; set; }
        public int PrintOrder { get; set; }
        public string FileName { get; set; }
        public string StoredFileName { get; set; }
        public string FilePath { get; set; }
        public DateTime Created { get; set; }
        public int CreatedById { get; set; }
        public int OrganizationId { get; set; }

        public int Identifiable => this.InvoiceAttachmentID;
    }
}