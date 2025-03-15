using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.UserViewModels
{
  public class UpdateEmailViewModel
  {
    public int UserID { get; set; }
    public string? CurrentEmail { get; set; }
    public string? NewEmail { get; set; }
    public ValidationResult? ValidationResult { get; set; }

    public class UpdateEmailViewModelValidator : AbstractValidator<UpdateEmailViewModel>
    {
      public UpdateEmailViewModelValidator()
      {
        RuleFor(x => x.NewEmail)
            .NotEmpty().WithMessage("New email is required")
            .EmailAddress().WithMessage("Invalid email format");
      }
    }
  }
}