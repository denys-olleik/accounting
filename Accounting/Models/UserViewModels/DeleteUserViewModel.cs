using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.UserViewModels
{
  public class DeleteUserViewModel
  {
    public int UserID { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsUserInUseAsync { get; set; }
    public bool DeleteReferences { get; set; }

    public ValidationResult ValidationResult { get; set; } = new();

    public class DeleteUserViewModelValidator : AbstractValidator<DeleteUserViewModel>
    {
      public DeleteUserViewModelValidator()
      {
        RuleFor(user => user)
          .Must(user => IsValidForDeletion(user))
          .WithMessage("This user has dependent records. Select 'Delete references' to proceed with deletion.");
      }

      private bool IsValidForDeletion(DeleteUserViewModel user)
      {
        if (user.IsUserInUseAsync)
        {
          return user.DeleteReferences;
        }
        return true;
      }
    }
  }
}