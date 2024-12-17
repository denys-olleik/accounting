using FluentValidation.Results;

namespace Accounting.Models.UserViewModels
{
  public class CreateUserViewModel
  {
    public string Email { get; set; }
    public string FirstName { get;  set; }
    public string LastName { get; set; }
    public bool SendInvitationEmail { get; set; }
    public ValidationResult ValidationResult { get; set; }
  }
}