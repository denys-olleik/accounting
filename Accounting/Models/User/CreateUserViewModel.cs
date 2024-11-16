using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.UserViewModels
{
  public class CreateUserViewModel
  {
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public bool SendInvitationEmail { get; set; }

    public ValidationResult ValidationResult { get; set; }
    public int TenantId { get; set; }

    public class CreateUserViewModelValidator : AbstractValidator<CreateUserViewModel>
    {
      public CreateUserViewModelValidator()
      {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
      }
    }
  }
}