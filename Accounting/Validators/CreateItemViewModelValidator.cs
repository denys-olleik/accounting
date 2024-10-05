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

      When(x => x.SellFor > 0, () =>
      {
        RuleFor(x => x.SelectedRevenueAccountId)
            .NotEmpty().WithMessage("Revenue account is required.")
            .DependentRules(() =>
            {
              RuleFor(x => x.SelectedRevenueAccountId)
                    .MustAsync(async (accountId, cancellation) => await AccountExists(accountId, _organizationId, cancellation))
                    .WithMessage("Selected revenue account does not exist.");
            });

        RuleFor(x => x.SelectedAssetsAccountId)
            .NotEmpty().WithMessage("Asset account is required.")
            .DependentRules(() =>
            {
              RuleFor(x => x.SelectedAssetsAccountId)
                    .MustAsync(async (accountId, cancellation) => await AccountExists(accountId, _organizationId, cancellation))
                    .WithMessage("Selected asset account does not exist.");
            });
      });

      RuleFor(x => x.SelectedItemType)
          .NotEmpty().WithMessage("Item type is required.");
    }

    private async Task<bool> AccountExists(int? accountId, int organizationId, CancellationToken cancellationToken)
    {
      if (!accountId.HasValue)
        return false;

      AccountService accountService = new AccountService();
      return await accountService.ExistsAsync(accountId.Value, organizationId);
    }
  }
}