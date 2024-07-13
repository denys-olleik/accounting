using Accounting.Models.TagViewModels;
using Accounting.Service;
using FluentValidation;

namespace Accounting.Validators
{
    public class CreateTagApiViewModelValidator : AbstractValidator<CreateTagApiViewModel>
    {
        public CreateTagApiViewModelValidator()
        {
            TagService tagService = new TagService();

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("'Name' is required.")
                .MustAsync(async (name, cancellation) =>
                {
                    var existingTag = await tagService.GetByNameAsync(name);
                    return existingTag == null;
                }).WithMessage((x, _) => $"A tag '{x.Name}' already exists.");
        }
    }
}