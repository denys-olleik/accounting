using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.LocationViewModels
{
  public class DeleteLocationViewModel
  {
    public int LocationID { get; set; }
    public string Name { get; set; }
    public bool IsLocationInUseAsync { get; set; }
    public bool DeleteChildren { get; set; }

    public ValidationResult ValidationResult { get; set; } = new();

    public class DeleteLocationViewModelValidator : AbstractValidator<DeleteLocationViewModel>
    {
      public DeleteLocationViewModelValidator()
      {
        RuleFor(location => location)
          .Must(location => IsValidForDeletion(location))
          .WithMessage("This location has dependent records. Select 'Delete children and references' to proceed with deletion.");
      }
      private bool IsValidForDeletion(DeleteLocationViewModel location)
      {
        // If the location has children, DeleteChildren must be true
        if (location.IsLocationInUseAsync)
        {
          return location.DeleteChildren;
        }
        // If the location does not have children, the rule is satisfied
        return true;
      }
    }
  }
}