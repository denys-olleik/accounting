using Accounting.Models.ItemViewModels;
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

      RuleFor(x => x.SelectedRevenueChartOfAccountId)
          .NotEmpty().WithMessage("Revenue account is required.")
          .DependentRules(() =>
          {
            RuleFor(x => x.SelectedRevenueChartOfAccountId)
                      .MustAsync(async (chartOfAccountId, cancellation) => await AccountExists(chartOfAccountId, _organizationId, cancellation))
                      .WithMessage("Selected revenue account does not exist.");
          });

      RuleFor(x => x.SelectedAssetsChartOfAccountId)
          .NotEmpty().WithMessage("Asset account is required.")
          .DependentRules(() =>
          {
            RuleFor(x => x.SelectedAssetsChartOfAccountId)
                      .MustAsync(async (chartOfAccountId, cancellation) => await AccountExists(chartOfAccountId, _organizationId, cancellation))
                      .WithMessage("Selected asset account does not exist.");
          });

      RuleFor(x => x.SelectedItemType)
          .NotEmpty().WithMessage("Item type is required.");

      // Conditional validation for Quantity and SalePrice
      When(x => x.SelectedLocationId.HasValue, () =>
      {
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity is required and must be greater than zero.");

        RuleFor(x => x.SalePrice)
            .GreaterThan(0).WithMessage("Sale Price is required and must be greater than zero.");
      });
    }

    private async Task<bool> AccountExists(int? chartOfAccountId, int organizationId, CancellationToken cancellationToken)
    {
      if (!chartOfAccountId.HasValue)
        return false;

      ChartOfAccountService chartOfAccountService = new ChartOfAccountService();
      return await chartOfAccountService.ExistsAsync(chartOfAccountId.Value, organizationId);
    }
  }
}
