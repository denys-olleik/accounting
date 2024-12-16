using FluentValidation;
using FluentValidation.Results;
using System.Collections.Generic;

namespace Accounting.Models.TenantViewModels
{
  public class CreateUserViewModel
  {
    public int TenantId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string SelectedOrganizationIdsCsv { get; set; }
    public ValidationResult ValidationResult { get; set; } = new ValidationResult();
    public UserViewModel? ExistingUser { get; set; }
    public List<OrganizationViewModel> AvailableOrganizations { get; internal set; }

    public class OrganizationViewModel
    {
      public int OrganizationID { get; set; }
      public string Name { get; set; }
    }

    public class UserViewModel
    {
      public int UserID { get; set; }
      public string Email { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public string Password { get; set; }
    }

    public class CreateUserViewModelValidator : AbstractValidator<CreateUserViewModel>
    {
      public CreateUserViewModelValidator()
      {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");

        When(x => x.ExistingUser == null, () =>
        {
          RuleFor(x => x.FirstName)
              .NotEmpty().WithMessage("First name is required.");

          RuleFor(x => x.LastName)
              .NotEmpty().WithMessage("Last name is required.");

          When(x => !string.IsNullOrEmpty(x.Password) || !string.IsNullOrEmpty(x.ConfirmPassword), () =>
          {
            RuleFor(x => x.Password)
                .Equal(x => x.ConfirmPassword).WithMessage("Password and Confirm Password must match.");
          });
        });

        When(x => x.ExistingUser != null, () =>
        {
          RuleFor(x => x.Password)
              .Empty().WithMessage("Password must be empty when an existing user is linked.");

          RuleFor(x => x.ConfirmPassword)
              .Empty().WithMessage("Confirm Password must be empty when an existing user is linked.");

          RuleFor(x => x.FirstName)
              .Empty().WithMessage("First name must be empty for an existing user.");

          RuleFor(x => x.LastName)
              .Empty().WithMessage("Last name must be empty for an existing user.");
        });
      }
    }
  }
}