using Accounting.Models.Tenant;
using Accounting.Service;
using FluentValidation;

namespace Accounting.Validators
{
  public class AddUserOrganizationViewModelValidator : AbstractValidator<AddUserOrganizationViewModel>
  {
    private readonly UserService _userService;
    private readonly OrganizationService _organizationService;
    private readonly string _tenantId;

    public AddUserOrganizationViewModelValidator(
      UserService userService,
      OrganizationService organizationService,
      string tenantId)
    {
      _userService = userService;
      _organizationService = organizationService;
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
                var existingUser = await _userService.GetAsync(model.Email!, false);
                return existingUser == null;
              }
              return true;
            }).WithMessage("A user with this email already exists. Inherit user instead.");
        });
    }
  }
}