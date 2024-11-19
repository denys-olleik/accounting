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
      private readonly UserService _userService;

      public CreateUserViewModelValidator(UserService userService)
      {
        _userService = userService;

        RuleFor(x => x.Email)
          .NotEmpty().WithMessage("Email is required.")
          .EmailAddress().WithMessage("Invalid email format.")
          .DependentRules(() =>
          {
            RuleFor(x => x)
              .MustAsync(async (model, cancellation) =>
              {
                var existingUser = await _userService.GetAsync(model.Email, model.TenantId);
                return existingUser == null;
              })
              .WithMessage("A user with this email already exists for the tenant.");
          });
      }
    }
  }
}