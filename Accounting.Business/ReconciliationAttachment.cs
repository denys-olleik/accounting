using Accounting.Common;

namespace Accounting.Business
{
  public class ReconciliationAttachment : IIdentifiable<int>
  {
    public int ReconciliationAttachmentID { get; set; }
    public int? ReconciliationId { get; set; }
    public string? OriginalFileName { get; set; }
    public string? FilePath { get; set; }
    public DateTime Created { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }

    public int Identifiable => this.ReconciliationAttachmentID;
  }
}