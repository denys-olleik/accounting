using Accounting.Models.LocationViewModels;
using FluentValidation;

namespace Accounting.Validators
{
  public class CreateLocationViewModelValidator : AbstractValidator<CreateLocationViewModel>
  {
    public CreateLocationViewModelValidator()
    {
      RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
    }
  }
}