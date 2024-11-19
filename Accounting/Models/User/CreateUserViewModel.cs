using Accounting.Database;
using Accounting.Service;
using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.UserViewModels
{
  public class CreateUserViewModel
  {
    public int TenantId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public bool SendInvitationEmail { get; set; }

    public ValidationResult ValidationResult { get; set; }
    public UserViewModel ExistingUser { get; set; }

    public class CreateUserViewModelValidator : AbstractValidator<CreateUserViewModel>
    {
      private readonly UserService _userService;

      public CreateUserViewModelValidator(UserService userService)
      {
        _userService = userService;

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is not valid.")
            .DependentRules(() =>
            {
              RuleFor(x => x.ExistingUser)
                  .Null()
                  .When(x => !string.IsNullOrWhiteSpace(x.FirstName) ||
                             !string.IsNullOrWhiteSpace(x.LastName) ||
                             !string.IsNullOrWhiteSpace(x.Password))
                  .WithMessage("User with this email exists elsewhere. Clear first, last, and password to reuse those properties.");
            });

        When(x => x.ExistingUser == null, () =>
        {
          RuleFor(x => x.FirstName)
              .NotEmpty().WithMessage("First name is required for new users.");

          RuleFor(x => x.LastName)
              .NotEmpty().WithMessage("Last name is required for new users.");

          RuleFor(x => x.Password)
              .NotEmpty().WithMessage("Password is required for new users.");

          RuleFor(x => x.ConfirmPassword)
              .Equal(x => x.Password)
              .WithMessage("Passwords do not match.");
        });

        When(x => x.ExistingUser != null, () =>
        {
          RuleFor(x => x.FirstName)
              .Empty().WithMessage("First name must be empty for existing users.");

          RuleFor(x => x.LastName)
              .Empty().WithMessage("Last name must be empty for existing users.");

          RuleFor(x => x.Password)
              .Empty().WithMessage("Password must be empty for existing users.");
        });
      }
    }
  }
}