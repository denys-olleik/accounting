using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.TenantViewModels
{
  public class UpdateUserViewModel
  {
    public int TenantId { get; set; }
    public int UserID { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<OrganizationViewModel> AvailableOrganizations { get; set; } = new List<OrganizationViewModel>();
    public string SelectedOrganizationIdsCsv { get; set; }
    public UserOrganizationViewModel ExistingUserOrganization { get; set; }

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
            .WithMessage("You cannot un-associate your own user-organization.");
      }

      /// <summary>
      /// Validates that the current user's organization association is not removed
      /// during the update process by ensuring the user's organization ID remains
      /// in the list of selected organization IDs.
      /// </summary>
      private bool NotUnassociateCurrentUser(UpdateUserViewModel model)
      {
        if (model.ExistingUserOrganization != null && model.UserID == model.ExistingUserOrganization.UserId)
        {
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