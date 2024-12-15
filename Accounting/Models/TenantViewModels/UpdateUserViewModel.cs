using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.TenantViewModels
{
  public class UpdateUserViewModel
  {
    public string CurrentUserDatabaseName { get; set; }
    public int TenantId { get; set; }
    public TenantViewModel Tenant { get; set; }
    public int UserID { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<OrganizationViewModel> AvailableOrganizations { get; set; } = new List<OrganizationViewModel>();
    public string SelectedOrganizationIdsCsv { get; set; }
    public UserOrganizationViewModel ExistingUserOrganization { get; set; }

    public class TenantViewModel
    {
      public int TenantID { get; set; }
      public string? DatabaseName { get; set; }
    }

    public class UserOrganizationViewModel
    {
      public int UserOrganizationID { get; set; }
      public int UserId { get; set; }
      public int OrganizationId { get; set; }
    }

    public ValidationResult? ValidationResult { get; set; } = new ValidationResult();

    public class OrganizationViewModel
    {
      public int OrganizationID { get; set; }
      public string Name { get; set; }
    }

    public class UpdateUserViewModelValidator : AbstractValidator<UpdateUserViewModel>
    {
      public UpdateUserViewModelValidator()
      {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.");

        RuleFor(x => x)
            .Must(NotUnassociateCurrentUser)
            .WithMessage("You cannot un-associate yourself from the organization you are logged into.")
            .When(x => x.CurrentUserDatabaseName == x.Tenant.DatabaseName);
      }

      private bool NotUnassociateCurrentUser(UpdateUserViewModel model)
      {
        if (model.ExistingUserOrganization != null && model.UserID == model.ExistingUserOrganization.UserId)
        {
          if (string.IsNullOrWhiteSpace(model.SelectedOrganizationIdsCsv))
          {
            return false;
          }

          var selectedOrgIds = model.SelectedOrganizationIdsCsv.Split(',')
              .Select(id => int.Parse(id.Trim()))
              .ToList();

          return selectedOrgIds.Contains(model.ExistingUserOrganization.OrganizationId);
        }

        return true;
      }
    }
  }
}