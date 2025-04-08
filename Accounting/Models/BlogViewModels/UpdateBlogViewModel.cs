using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.BlogViewModels
{
  public class UpdateBlogViewModel
  {
    public int BlogID { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public bool Public { get; set; }

    public ValidationResult ValidationResult { get; set; } = new();

    public class UpdateBlogViewModelValidator : AbstractValidator<UpdateBlogViewModel>
    {
      public UpdateBlogViewModelValidator()
      {
        RuleFor(x => x.Title)
          .NotEmpty().WithMessage("Title is required.");

        RuleFor(x => x.Content)
          .NotEmpty().WithMessage("Content is required.");
      }
    }
  }
}