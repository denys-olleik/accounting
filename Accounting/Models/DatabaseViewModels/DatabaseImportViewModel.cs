using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.DatabaseViewModels
{
  public class DatabaseImportViewModel
  {
    private string? _email;
    private string? _databaseName;
    public ValidationResult? ValidationResult { get; set; }

    public string? Email
    {
      get => _email;
      set => _email = value?.Trim();
    }

    public IFormFile? UploadedFile { get; set; }

    public class DatabaseImportViewModelValidator : AbstractValidator<DatabaseImportViewModel>
    {
      public DatabaseImportViewModelValidator()
      {
        RuleFor(x => x.Email)
          .NotEmpty()
          .EmailAddress()
          .WithMessage("Invalid email address.");
      }
    }
  }
}