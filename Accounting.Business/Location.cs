using Accounting.Common;

namespace Accounting.Business
{
  public class Location : IIdentifiable<int>
  {
    public int LocationID { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime Created { get; set; }
    public int? ParentLocationId { get; set; }
    public int CreatedById { get; set; }
    public object? OrganizationId { get; set; }

    public List<Location>? Children { get; set; }

    public int Identifiable => this.LocationID;
  }
}