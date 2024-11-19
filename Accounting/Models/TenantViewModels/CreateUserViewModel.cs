using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.TenantViewModels
{
  public class CreateUserViewModel
  {
    public int TenantId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }

    public ValidationResult ValidationResult { get; set; }
    public ExistingUserViewModel ExistingUser { get; set; }

    public class ExistingUserViewModel
    {
      public int UserID { get; set; }
      public string Email { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public string Password { get; set; }
    }

    public class CreateUserViewModelValidator : AbstractValidator<CreateUserViewModel>
    {
      public CreateUserViewModelValidator()
      {
       
      }
    }
  }
}