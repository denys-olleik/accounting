using FluentValidation.Results;

namespace Accounting.Models.RegistrationViewModels
{
  public class RegisterViewModel
  {
    public string Email { get; set; }

    public ValidationResult ValidationResult { get; set; } = new();
  }
}