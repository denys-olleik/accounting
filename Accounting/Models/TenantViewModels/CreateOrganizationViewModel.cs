using Accounting.Business;
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
      private readonly string _databaseName;

      public CreateOrganizationViewModelValidator(OrganizationService organizationService, string databaseName)
      {
        _organizationService = organizationService;
        _databaseName = databaseName;

        RuleFor(x => x.Name)
          .NotEmpty()
          .WithMessage("Name is required")
          .DependentRules(() =>
          {
            RuleFor(x => x)
                .MustAsync(async (model, cancellation) =>
                    !await OrganizationExistsAsync(model.Name, _databaseName))
                .WithMessage("Organization with this name already exists for this tenant");
          });

        RuleFor(x => x.TenantId)
          .NotEmpty()
          .WithMessage("TenantId is required");
      }

      private async Task<bool> OrganizationExistsAsync(string name, string databaseName)
      {
        Organization organization = await _organizationService.GetAsync(name, databaseName);
        return organization != null;
      }
    }
  }
}