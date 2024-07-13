using Accounting.Business;
using Accounting.Models.ChartOfAccount;
using Accounting.Service;
using FluentValidation;

namespace Accounting.Validators
{
  public class UpdateChartOfAccountViewModelValidator : AbstractValidator<UpdateChartOfAccountViewModel>
  {
    private readonly ChartOfAccountService _chartOfAccountService;

    public UpdateChartOfAccountViewModelValidator(ChartOfAccountService chartOfAccountService, int organizationId)
    {
      _chartOfAccountService = chartOfAccountService;

      RuleFor(x => x.ChartOfAccountID)
          .NotEmpty().WithMessage("'Chart Of Account ID' is required.");

      RuleFor(x => x.AccountName)
          .NotEmpty().WithMessage("'Account Name' is required.")
          .MaximumLength(200).WithMessage("'Account Name' cannot be longer than 200 characters.")
          .MustAsync(async (model, accountName, cancellation) =>
              await BeUniqueAccountName(model.ChartOfAccountID, accountName, organizationId))
          .WithMessage("Account Name must be unique within the organization.");

      RuleFor(x => x.SelectedAccountType)
          .NotEmpty().WithMessage("'Account Type' is required.")
          .MaximumLength(50).WithMessage("'Account Type' cannot be longer than 50 characters.")
          .Must(x => ChartOfAccount.AccountTypeConstants.All.Contains(x)).WithMessage("'Account Type' is invalid.");
    }

    private async Task<bool> BeUniqueAccountName(int chartOfAccountId, string accountName, int organizationId)
    {
      var result = await _chartOfAccountService.GetByAccountNameAsync(accountName, organizationId);
      return result == null || result.ChartOfAccountID == chartOfAccountId;
    }
  }
}