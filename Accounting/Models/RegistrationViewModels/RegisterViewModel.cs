using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.RegistrationViewModels
{
  public class RegisterViewModel
  {
    public string? Email { get; set; }
    public string? Password { get; set; }
    public bool Shared { get; set; }

    public ValidationResult ValidationResult { get; set; } = new();

    public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
    {
      public RegisterViewModelValidator()
      {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
      }
    }
  }
}