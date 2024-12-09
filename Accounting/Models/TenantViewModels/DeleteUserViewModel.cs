using FluentValidation.Results;

namespace Accounting.Models.TenantViewModels
{
  public class DeleteUserViewModel
  {
    public int TenantId { get; set; }
    public int UserId { get; set; }

    public ValidationResult ValidationResult { get; set; } = new ValidationResult();
  }
}