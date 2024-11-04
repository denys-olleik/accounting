using FluentValidation.Results;

namespace Accounting.Models.UserAccountViewModels
{
  public class CompleteLoginWithoutPasswordViewModel
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
    public string? Code { get; set; }
    
    public ValidationResult ValidationResult { get; set; } = new ValidationResult();
  }
}