using FluentValidation.Results;

namespace Accounting.Models.TenantViewModels
{
  public class ProvisionTenantViewModel
  {
    public string? Email { get; set; }
    public string? Name { get; set; }
    public int OrganizationId { get; set; }

    public ValidationResult? ValidationResult { get; set; }
  }
}