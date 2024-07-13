namespace Accounting.Models.LocationViewModels
{
  public class LocationViewModel
  {
    public int LocationID { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime Created { get; set; }
    public int? ParentLocationId { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }
    public List<LocationViewModel>? Children { get; set; }
  }
}