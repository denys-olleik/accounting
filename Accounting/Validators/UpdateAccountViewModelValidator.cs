using Accounting.Business;
using Accounting.Models.Account;
using Accounting.Service;
using FluentValidation;

namespace Accounting.Validators
{
  public class UpdateAccountViewModelValidator : AbstractValidator<UpdateAccountViewModel>
  {
    private readonly AccountService _accountService;

    public UpdateAccountViewModelValidator(AccountService accountService, int organizationId)
    {
      _accountService = accountService;

      RuleFor(x => x.AccountID)
          .NotEmpty().WithMessage("'Account ID' is required.");

      RuleFor(x => x.AccountName)
          .NotEmpty().WithMessage("'Account Name' is required.")
          .MaximumLength(200).WithMessage("'Account Name' cannot be longer than 200 characters.")
          .MustAsync(async (model, accountName, cancellation) =>
              await BeUniqueAccountName(model.AccountID, accountName, organizationId))
          .WithMessage("Account Name must be unique within the organization.");

      RuleFor(x => x.SelectedAccountType)
          .NotEmpty().WithMessage("'Account Type' is required.")
          .MaximumLength(50).WithMessage("'Account Type' cannot be longer than 50 characters.")
          .Must(x => Account.AccountTypeConstants.All.Contains(x)).WithMessage("'Account Type' is invalid.");
    }

    private async Task<bool> BeUniqueAccountName(int accountId, string accountName, int organizationId)
    {
      var result = await _accountService.GetByAccountNameAsync(accountName, organizationId);
      return result == null || result.AccountID == accountId;
    }
  }
}