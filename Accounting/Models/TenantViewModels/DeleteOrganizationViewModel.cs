using FluentValidation.Results;

namespace Accounting.Models.TenantViewModels
{
  public class DeleteOrganizationViewModel
  {
    public int TenantId { get; set; }
    public int OrganizationId { get; set; }
    public string OrganizationName { get; set; }

    public ValidationResult ValidationResult { get; set; } = new ValidationResult();
  }
}