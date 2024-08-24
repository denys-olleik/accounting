using FluentValidation.Results;

namespace Accounting.Models.TenantViewModels
{
  public class ProvisionTenantViewModel
  {
    public string? Email { get; set; }

    public ValidationResult? ValidationResult { get; set; }
  }
}