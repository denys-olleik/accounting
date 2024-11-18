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
              RuleFor(x => x)
                  .MustAsync(async (model, cancellation) =>
                  {
                    var (existingUser, _) = await _userService.GetAsync(model.Email);
                    return existingUser == null ||
                             (string.IsNullOrWhiteSpace(model.FirstName) &&
                              string.IsNullOrWhiteSpace(model.LastName) &&
                              string.IsNullOrWhiteSpace(model.Password));
                  })
                  .WithMessage("User with this email exists elsewhere, clear first, last, and password to reuse those properties.");
            });

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.");

        RuleFor(x => x.Password)
            .NotEmpty().When(x => !string.IsNullOrWhiteSpace(x.Password))
            .WithMessage("Password cannot be empty if provided.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .When(x => !string.IsNullOrWhiteSpace(x.Password))
            .WithMessage("Passwords do not match.");
      }
    }
  }
}