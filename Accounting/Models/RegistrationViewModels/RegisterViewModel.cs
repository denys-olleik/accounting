using FluentValidation.Results;

namespace Accounting.Models.RegistrationViewModels
{
  public class RegisterViewModel
  {
    public string? Email { get; set; }
    public string? Password { get; set; }
    public bool Shared { get; set; }

    public ValidationResult ValidationResult { get; set; } = new();
  }
}