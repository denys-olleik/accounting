using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.TenantViewModels
{
  public class UpdateOrganizationViewModel
  {
    public int TenantId { get; set; }
    public int OrganizationID { get; set; }
    public string Name { get; set; }

    public ValidationResult ValidationResult { get; set; } = new ValidationResult();

    public class UpdateOrganizationViewModelValidator : AbstractValidator<UpdateOrganizationViewModel>
    {
      public UpdateOrganizationViewModelValidator()
      {
        RuleFor(x => x.Name)
          .NotEmpty()
          .WithMessage("Organization name is required.")
          .MaximumLength(100)
          .WithMessage("Organization name cannot exceed 100 characters.");
      }
    }
  }
}