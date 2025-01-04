using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.RegistrationViewModels
{
  public class RegisterViewModel
  {
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Password { get; set; }
    public bool Shared { get; set; }

    public ValidationResult ValidationResult { get; set; } = new();

    public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
    {
      public RegisterViewModelValidator()
      {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Valid email is required");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");

        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required");
      }
    }
  }
}