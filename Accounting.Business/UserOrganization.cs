using Accounting.Common;

namespace Accounting.Business
{
  public class UserOrganization : IIdentifiable<int>
  {
    public int UserOrganizationID { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    public DateTime Created { get; set; }

    public int Identifiable => this.UserOrganizationID;
  }
}