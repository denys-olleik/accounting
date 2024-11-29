using Accounting.Models.Item;
using Accounting.Service;
using FluentValidation;

namespace Accounting.Validators
{
  public class CreateItemViewModelValidator : AbstractValidator<CreateItemViewModel>
  {
    private readonly int _organizationId;

    public CreateItemViewModelValidator(int organizationId)
    {
      _organizationId = organizationId;

      RuleFor(x => x.Name)
          .NotEmpty().WithMessage("Name is required.")
          .MaximumLength(100).WithMessage("Name cannot be more than 100 characters.");

      RuleFor(x => x.SelectedRevenueAccountId)
          .NotEmpty().WithMessage("Revenue account is required.");

      RuleFor(x => x.SelectedAssetsAccountId)
          .NotEmpty().WithMessage("Asset account is required.");

      RuleFor(x => x.SelectedItemType)
          .NotEmpty().WithMessage("Item type is required.");
    }
  }
}