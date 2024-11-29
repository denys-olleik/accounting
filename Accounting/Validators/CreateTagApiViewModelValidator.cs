using Accounting.Models.TagViewModels;
using FluentValidation;

namespace Accounting.Validators
{
  public class CreateTagApiViewModelValidator : AbstractValidator<CreateTagApiViewModel>
  {
    public CreateTagApiViewModelValidator()
    {
      RuleFor(x => x.Name)
          .NotEmpty().WithMessage("'Name' is required.")
          .MustAsync(async (name, cancellation) =>
          {
            throw new NotImplementedException();
          }).WithMessage((x, _) => $"A tag '{x.Name}' already exists.");
    }
  }
}