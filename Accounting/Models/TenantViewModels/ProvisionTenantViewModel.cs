using Accounting.Service;
using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.TenantViewModels
{
  public class ProvisionTenantViewModel
  {
    public string? Email { get; set; }
    public bool Shared { get; set; }
    public string? FullyQualifiedDomainName { get; set; }
    public string? ApplicationName { get; set; }
    public string? DefaultNoReplyEmailAddress { get; set; }
    public string? NoReplyEmailAddress { get; set; }

    public ValidationResult? ValidationResult { get; set; } = new();
    public bool EnableTenantManagement { get; set; }

    public class ProvisionTenantViewModelValidator
     : AbstractValidator<ProvisionTenantViewModel>
    {
      private readonly TenantService _tenantService;

      public ProvisionTenantViewModelValidator(
          TenantService tenantService,
          SecretService secretService)
      {
        _tenantService = tenantService;

        RuleFor(x => x.Email)
          .NotEmpty()
          .WithMessage("Email is required.")
          .DependentRules(() =>
          {
            RuleFor(x => x.Email)
              .EmailAddress()
              .WithMessage("Invalid email format.")
              .DependentRules(() =>
              {
                RuleFor(x => x)
                  .MustAsync(async (model, cancellation) =>
                  {
                    return !await _tenantService.ExistsAsync(model.Email!);
                  })
                  .WithMessage("A tenant with this email already exists.");
              });
          });

        RuleFor(x => x.FullyQualifiedDomainName)
          .NotEmpty()
          .When(x => !x.Shared)
          .WithMessage("'Fully Qualified Domain Name' is required when 'Shared' is not selected.");

        RuleFor(x => x.EnableTenantManagement)
          .Equal(false)
          .When(x => x.Shared)
          .WithMessage("'Enable tenant management' (non-shared instances only).");
      }
    }
  }
}