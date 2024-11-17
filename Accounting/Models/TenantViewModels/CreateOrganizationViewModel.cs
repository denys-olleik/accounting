using Accounting.Service;
using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.TenantViewModels
{
  public class CreateOrganizationViewModel
  {
    public string Name { get; set; }
    public int TenantId { get; set; }

    public ValidationResult ValidationResult { get; set; }
      = new ValidationResult();

    public class CreateOrganizationViewModelValidator : AbstractValidator<CreateOrganizationViewModel>
    {
      private readonly OrganizationService _organizationService;

      public CreateOrganizationViewModelValidator(OrganizationService organizationService)
      {
        _organizationService = organizationService;

        RuleFor(x => x.Name)
          .NotEmpty()
          .WithMessage("Name is required")
          .DependentRules(() =>
          {
            RuleFor(x => x)
                .MustAsync(async (model, cancellation) =>
                    !await OrganizationExistsAsync(model.TenantId, model.Name))
                .WithMessage("Organization with this name already exists for this tenant");
          });


        RuleFor(x => x.TenantId)
          .NotEmpty()
          .WithMessage("TenantId is required");
      }

      private async Task<bool> OrganizationExistsAsync(int tenantId, string name)
      {
        OrganizationService organization = await _organizationService.GetAsync(name, tenantId);
      }
    }
  }
}