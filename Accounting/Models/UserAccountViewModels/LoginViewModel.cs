using FluentValidation.Results;

namespace Accounting.Models.UserAccountViewModels
{
  public class LoginViewModel
  {
    public string Email { get; set; }
    public string Password { get; set; }
    public ValidationResult ValidationResult { get; set; }
  }
}