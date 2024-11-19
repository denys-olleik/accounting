using Accounting.Service;
using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.TenantViewModels
{
  public class CreateUserViewModel
  {
    public int TenantId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }

    public ValidationResult ValidationResult { get; set; }
    public UserViewModel ExistingUser { get; set; }

    public class UserViewModel
    {
      public int UserID { get; set; }
      public string Email { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public string Password { get; set; }
    }

    public class CreateUserViewModelValidator : AbstractValidator<CreateUserViewModel>
    {
      public CreateUserViewModelValidator()
      {
        RuleFor(x => x.Email)
          .NotEmpty().WithMessage("Email is required.")
          .EmailAddress().WithMessage("Invalid email format.")
          .DependentRules(() =>
          {
            RuleFor(x => x)
              .Must(model =>
              {
                if (model.ExistingUser != null)
                {
                  return string.IsNullOrWhiteSpace(model.FirstName) && string.IsNullOrWhiteSpace(model.LastName);
                }
                return true;
              })
              .WithMessage("First name and last name must be blank if an existing user with this email already exists.");
          });

        RuleFor(x => x.FirstName)
          .NotEmpty().WithMessage("First name is required.")
          .When(model => model.ExistingUser == null);

        RuleFor(x => x.LastName)
          .NotEmpty().WithMessage("Last name is required.")
          .When(model => model.ExistingUser == null);

        RuleFor(x => x.Password)
          .NotEmpty().WithMessage("Password is required.")
          .When(model => model.ExistingUser == null);

        RuleFor(x => x.ConfirmPassword)
          .NotEmpty().WithMessage("Password confirmation is required.")
          .Equal(x => x.Password).WithMessage("Passwords must match.")
          .When(model => model.ExistingUser == null);
      }
    }
  }
}