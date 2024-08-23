using Accounting.Models.TenantViewModels;
using FluentValidation;
using Accounting.Service;

namespace Accounting.Validators
{
  public class CreateTenantViewModelValidator : AbstractValidator<CreateTenantViewModel>
  {
    public CreateTenantViewModelValidator(TenantService tenantService)
    {
      RuleFor(x => x.Email)
          .NotEmpty()
          .EmailAddress()
          .MustAsync(async (email, cancellation) =>
          {
            return !await tenantService.ExistsAsync(email);
          }).WithMessage("A tenant with this email already exists.");
    }
  }
}