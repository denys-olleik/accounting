using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.UserViewModels
{
  public class CreateUserViewModel
  {
    public string? Email { get; set; }

    public ValidationResult ValidationResult { get; set; } = new();

    public class CreateUserViewModelValidator : AbstractValidator<CreateUserViewModel>
    {
      public CreateUserViewModelValidator()
      {
        RuleFor(x => x.Email)
          .Cascade(CascadeMode.Stop)
          .NotEmpty()
          .EmailAddress();
      }
    }
  }
}