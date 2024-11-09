using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.TenantViewModels
{
  public class TenantLoginViewModel
  {
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? OrganizationPublicId { get; set; }
    public string? TenantPublicId { get; set; }

    public ValidationResult? ValidationResult { get; set; }

    public class TenantLoginViewModelValidator
    : AbstractValidator<TenantLoginViewModel>
    {
      public TenantLoginViewModelValidator()
      {
        RuleFor(x => x.Email)
          .NotEmpty()
          .WithMessage("Email is required.")
          .DependentRules(() =>
          {
            RuleFor(x => x.Password)
              .NotEmpty()
              .WithMessage("Password is required.");
          });
      }
    }
  }
}