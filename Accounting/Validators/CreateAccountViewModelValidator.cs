using Accounting.Business;
using Accounting.Models.Account;
using Accounting.Service;
using FluentValidation;

namespace Accounting.Validators
{
  public class CreateAccountViewModelValidator : AbstractValidator<CreateAccountViewModel>
  {
    private readonly AccountService _accountService;

    public CreateAccountViewModelValidator(AccountService accountService)
    {
      _accountService = accountService;

      RuleFor(x => x.AccountName)
          .NotEmpty().WithMessage("'Account Name' is required.")
          .MaximumLength(200).WithMessage("'Account Name' cannot be longer than 200 characters.");

      RuleFor(x => x.SelectedAccountType)
          .Must(BeAValidAccountType)
          .WithMessage("'Account Type' is required.");

      RuleFor(x => x)
          .MustAsync(MatchParentAccountTypeAsync)
          .WithMessage("Child account type must match parent account type.")
          .When(x => x.ParentAccountId.HasValue);
    }

    private bool BeAValidAccountType(string? accountType)
    {
      return !string.IsNullOrEmpty(accountType) && Account.AccountTypeConstants.All.Contains(accountType);
    }

    private async Task<bool> MatchParentAccountTypeAsync(CreateAccountViewModel model, CancellationToken cancellationToken)
    {
      var parentAccountType = await _accountService.GetTypeAsync(model.ParentAccountId!.Value);
      return parentAccountType == model.SelectedAccountType;
    }
  }
}