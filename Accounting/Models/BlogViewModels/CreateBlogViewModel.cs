using FluentValidation.Results;
using FluentValidation;

namespace Accounting.Models.BlogViewModels
{
  public class CreateBlogViewModel
  {
    public string? Title { get; set; }
    public string? Content { get; set; }
    public bool Public { get; set; }

    public ValidationResult ValidationResult { get; set; } = new();

    public class CreateBlogViewModelValidator : AbstractValidator<CreateBlogViewModel>
    {
      public CreateBlogViewModelValidator()
      {
        RuleFor(x => x.Title)
          .NotEmpty().WithMessage("Title is required.");

        RuleFor(x => x.Content)
          .NotEmpty().WithMessage("Content is required.");
      }
    }
  }
}