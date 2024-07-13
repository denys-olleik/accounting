using FluentValidation.Results;

namespace Accounting.Models.LocationViewModels
{
  public class CreateLocationViewModel
  {
    public int? ParentLocationId { get; set; }
    public LocationViewModel? ParentLocation { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public ValidationResult? ValidationResult { get; set; }

    public class LocationViewModel
    {
      public int LocationId { get; set; }
      public string? Name { get; set; }
    }
  }
}