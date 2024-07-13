using Accounting.Business;
using Accounting.Models.ChartOfAccount;
using FluentValidation;

namespace Accounting.Validators
{
  public class CreateChartOfAccountViewModelValidator : AbstractValidator<CreateChartOfAccountViewModel>
  {
    public CreateChartOfAccountViewModelValidator()
    {
      RuleFor(x => x.AccountName)
        .NotEmpty().WithMessage("'Account Name' is required.")
        .MaximumLength(200).WithMessage("'Account Name' cannot be longer than 200 characters.");

      RuleFor(x => x.SelectedAccountType)
        .Must(BeAValidAccountType)
        .WithMessage("'Account Type' is required.");
    }

    private bool BeAValidAccountType(string? accountType)
    {
      return !string.IsNullOrEmpty(accountType) && ChartOfAccount.AccountTypeConstants.All.Contains(accountType);
    }
  }
}