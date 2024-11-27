using Accounting.Service;
using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.TenantViewModels
{
  public class AddUserOrganizationViewModel
  {
    public int TenantId { get; set; }

    private string? _email;
    public string? Email
    {
      get => _email;
      set => _email = value?.Trim().ToLower();
    }

    public bool InheritUser { get; set; }

    private string? _firstName;
    public string? FirstName
    {
      get => _firstName;
      set => _firstName = value?.Trim();
    }

    private string? _lastName;
    public string? LastName
    {
      get => _lastName;
      set => _lastName = value?.Trim();
    }

    private string? _password;
    public string? Password
    {
      get => _password;
      set => _password = value?.Trim();
    }

    private string? _organizationName;
    public string? OrganizationName
    {
      get => _organizationName;
      set => _organizationName = value?.Trim();
    }

    public bool InheritOrganization { get; set; }

    public ValidationResult? ValidationResult { get; set; } = new ValidationResult();

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
                var (existingUser, tenantExistingUserBelongsTo) = await _userService.GetFirstOfAnyTenantAsync(model.Email!);
                return model.InheritUser ? existingUser != null : existingUser == null;
              })
              .WithMessage(model =>
                model.InheritUser ? "User does not exist to inherit." : "A user with this email already exists. Inherit user instead.");
          });

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .When(x => !x.InheritUser);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .When(x => !x.InheritUser);

        RuleFor(x => x.OrganizationName)
            .NotEmpty().WithMessage("Organization name is required.")
            .DependentRules(() =>
            {
              RuleFor(x => x)
                  .MustAsync(async (model, cancellationToken) =>
                  {
                    var tenant = await _tenantService.GetAsync(Convert.ToInt32(_tenantId));
                    var existingOrganization = await _organizationService.GetAsync(model.OrganizationName!, tenant.DatabaseName);
                    return model.InheritOrganization ? existingOrganization != null : existingOrganization == null;
                  })
                  .WithMessage(model =>
                      model.InheritOrganization ? "Organization does not exist to inherit." : "This organization already exists. Inherit organization instead.");
            });
      }
    }
  }
}