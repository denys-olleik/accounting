using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.DatabaseViewModels
{
  public class DatabaseImportViewModel
  {
    public ValidationResult? ValidationResult { get; set; }

    public IFormFile? DatabaseBackup { get; set; }
  }
}