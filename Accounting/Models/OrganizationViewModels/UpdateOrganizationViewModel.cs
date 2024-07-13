using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Accounting.Models.OrganizationViewModels
{
  public class UpdateOrganizationViewModel
  {
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? AccountsReceivableEmail { get; set; }
    public string? AccountsPayableEmail { get; set; }
    public string? AccountsReceivablePhone { get; set; }
    public string? AccountsPayablePhone { get; set; }
    public string? Website { get; set; }

    public ValidationResult? ValidationResult { get; set; }
  }
}