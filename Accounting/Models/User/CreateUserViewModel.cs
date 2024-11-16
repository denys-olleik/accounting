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

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("TenantId is required.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is not in email format.")
            .DependentRules(() =>
            {
              RuleFor(x => x)
                  .MustAsync(async (model, cancellationToken) =>
                      !await UserExistsAsync(model.Email, model.TenantId))
                  .WithMessage("A user with this email already exists for the specified tenant.");
            });
      }

      private async Task<bool> UserExistsAsync(string email, int tenantId)
      {
        var existingUser = await _userService.GetAsync(email, tenantId);
        return existingUser != null;
      }
    }
  }
}