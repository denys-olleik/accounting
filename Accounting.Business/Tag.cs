using Accounting.Common;

namespace Accounting.Business
{
  public class Tag : IIdentifiable<int>
  {
    public int TagID { get; set; }
    public string Name { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }

    public int Identifiable => this.TagID;
  }
}