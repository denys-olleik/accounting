using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.OrganizationViewModels
{
  public class CreateOrganizationViewModel
  {
    public string? Name { get; set; }

    public ValidationResult ValidationResult { get; set; } = new();

    public class CreateOrganizationViewModelValidator : AbstractValidator<CreateOrganizationViewModel>
    {
      public CreateOrganizationViewModelValidator()
      {
        RuleFor(x => x.Name)
          .Cascade(CascadeMode.Stop)
          .NotEmpty();
      }
    }
  }
}