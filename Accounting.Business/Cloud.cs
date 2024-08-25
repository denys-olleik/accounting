using Accounting.Common;

namespace Accounting.Business
{
  public class Cloud : IIdentifiable<int>
  {
    public int CloudID { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime Created { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }

    public int Identifiable => this.CloudID;
  }
}
