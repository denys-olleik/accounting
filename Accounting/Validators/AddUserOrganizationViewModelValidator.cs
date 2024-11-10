using Accounting.Business;
using Accounting.Models.Tenant;
using Accounting.Service;
using FluentValidation;

namespace Accounting.Validators
{
  public class AddUserOrganizationViewModelValidator : AbstractValidator<AddUserOrganizationViewModel>
  {
    private readonly UserService _userService;
    private readonly OrganizationService _organizationService;
    private readonly TenantService _tenantService;
    private readonly string _tenantId;

    public AddUserOrganizationViewModelValidator(
      UserService userService,
      OrganizationService organizationService,
      TenantService tenantService,
      string tenantId)
    {
      _userService = userService;
      _organizationService = organizationService;
      _tenantService = tenantService;
      _tenantId = tenantId;

      RuleFor(x => x.Email)
        .NotEmpty().WithMessage("Email is required.")
        .EmailAddress().WithMessage("Invalid email format.")
        .DependentRules(() =>
        {
          RuleFor(x => x)
            .MustAsync(async (model, cancellationToken) =>
            {
              if (!model.InheritUser)
              {
                var existingUser = await _userService.GetAsync(model.Email!, true);
                return existingUser == null;
              }
              return true;
            }).WithMessage("A user with this email already exists. Inherit user instead.");
        });

      RuleFor(x => x.OrganizationName)
        .NotEmpty().WithMessage("Organization name is required.")
        .DependentRules(() =>
        {
          RuleFor(x => x)
            .MustAsync(async (model, cancellationToken) =>
            {
              if (!model.InheritOrganization)
              {
                Tenant tenant = await _tenantService.GetAsync(Convert.ToInt32(_tenantId));
                var existingOrganization = await _organizationService.GetAsync(model.OrganizationName!, tenant.DatabaseName);
                return existingOrganization == null;
              }
              return true;
            }).WithMessage("This combination of user and organization already exists.");
        });
    }
  }
}