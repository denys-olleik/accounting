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
    }
  }
}