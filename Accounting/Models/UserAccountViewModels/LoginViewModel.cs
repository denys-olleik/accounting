using FluentValidation.Results;

namespace Accounting.Models.UserAccountViewModels
{
  public class LoginViewModel
  {
    private string? email;
    public string? Email
    {
      get
      {
        return email;
      }
      set
      {
        email = value?.Trim()!.ToLower()!;
      }
    }
    public string? Password { get; set; }
    public ValidationResult? ValidationResult { get; set; }
  }
}