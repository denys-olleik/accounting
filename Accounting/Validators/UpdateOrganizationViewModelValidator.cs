using Accounting.Models.OrganizationViewModels;
using FluentValidation;
using static Accounting.Business.Organization;

namespace Accounting.Validators
{
  public class UpdateOrganizationViewModelValidator : AbstractValidator<UpdateOrganizationViewModel>
  {
    public UpdateOrganizationViewModelValidator()
    {
      RuleFor(x => x.Name).NotEmpty().WithMessage("Organization Name is required.");
    }
  }
}