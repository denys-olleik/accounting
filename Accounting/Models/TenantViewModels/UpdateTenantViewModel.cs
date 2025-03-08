using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.TenantViewModels
{
  public class UpdateTenantViewModel
  {
    public int TenantId { get; set; }
    private string? _email;
    public string? Email
    {
      get { return _email; }
      set { _email = value?.Trim(); }
    }
    public TenantViewModel? PotentialTenant { get; set; }
    public ValidationResult? ValidationResult { get; set; }

    public class TenantViewModel
    {
      public int TenantId { get; set; }
      public string? Email { get; set; }
    }

    public class UpdateTenantViewModelValidator : AbstractValidator<UpdateTenantViewModel>
    {
      public UpdateTenantViewModelValidator()
      {
        RuleFor(x => x.Email)
          .NotEmpty()
          .WithMessage("Email is required.")
          .EmailAddress()
          .WithMessage("Invalid email format.");

        RuleFor(x => x.Email)
          .Must((model, email) => model.PotentialTenant!.Email != email)
          .WithMessage("The email address must be different from the current one.")
          .When(x => x.PotentialTenant != null);
      }
    }
  }
}