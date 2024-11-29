using Accounting.Models.TagViewModels;

using Accounting.Service;
using FluentValidation;

namespace Accounting.Validators
{
  public class CreateTagViewModelValidator : AbstractValidator<CreateTagViewModel>
  {
    public CreateTagViewModelValidator()
    {
      RuleFor(x => x.Name).NotEmpty().MaximumLength(100)
          .MustAsync(TagDoesNotExistAsync).WithMessage(x => $"The tag '{x.Name}' already exists.");

      // Add this rule to ensure SelectedMatchType is selected
      RuleFor(x => x.SelectedMatchType).NotEmpty().WithMessage("You must select a match type.");
    }

    private async Task<bool> TagDoesNotExistAsync(string name, CancellationToken token)
    {
      throw new NotImplementedException();
    }
  }
}