using Accounting.Common;

namespace Accounting.Business
{
  public class Invitation : IIdentifiable<int>
  {
    public int InvitationID { get; set; }
    public Guid Guid { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int UserId { get; set; }
    public DateTime? Expiration { get; set; }
    public DateTime Created { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }
    public int Identifiable => this.InvitationID;
  }
}