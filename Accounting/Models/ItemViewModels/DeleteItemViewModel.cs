using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.ItemViewModels
{
  public class DeleteItemViewModel
  {
    public int ItemID { get; set; }
    public string? Name { get; set; }
    public bool HasChildren { get; set; }
    public bool DeleteChildren { get; set; }

    public ValidationResult ValidationResult { get; set; } = new();

    public class DeleteItemViewModelValidator : AbstractValidator<DeleteItemViewModel>
    {
      public DeleteItemViewModelValidator()
      {
        RuleFor(item => item)
          .Must(item => IsValidForDeletion(item))
          .WithMessage("Delete children first or select delete children to have all descendants .");
      }

      private bool IsValidForDeletion(DeleteItemViewModel item)
      {
        // If the item has children, DeleteChildren must be true
        if (item.HasChildren)
        {
          return item.DeleteChildren;
        }

        // If the item does not have children, the rule is satisfied
        return true;
      }
    }
  }
}