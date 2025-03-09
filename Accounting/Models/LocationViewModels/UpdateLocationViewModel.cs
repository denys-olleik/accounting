using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.LocationViewModels
{
  public class UpdateLocationViewModel
  {
    public int LocationId { get; set; }
    
    private string? name;
    public string? Name
    {
      get { return name; }
      set { name = value?.Trim(); }
    }

    public ValidationResult ValidationResult { get; set; } = new();

    public class UpdateLocationViewModelValidator : AbstractValidator<UpdateLocationViewModel>
    {
      public UpdateLocationViewModelValidator()
      {
        RuleFor(x => x.Name)
          .NotEmpty()
          .WithMessage("Name is required.");
      }
    }
  }
}