using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.ItemViewModels
{
  public class DeleteItemViewModel
  {
    public int ItemID { get; set; }
    public string? Name { get; set; }

    public ValidationResult ValidationResult { get; set; } = new ();
  }
}