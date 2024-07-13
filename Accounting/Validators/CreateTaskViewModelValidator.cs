using Accounting.Models.ToDoViewModels;
using FluentValidation;

namespace Accounting.Validators
{
  public class CreateTaskViewModelValidator : AbstractValidator<CreateToDoViewModel>
  {
    public CreateTaskViewModelValidator()
    {
      RuleFor(x => x.Title).NotEmpty().WithMessage("'Title' is required.");
    }
  }
}