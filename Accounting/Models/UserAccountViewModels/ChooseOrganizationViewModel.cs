using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Accounting.Models.UserAccountViewModels
{
  public class ChooseOrganizationViewModel
  {
    public List<SelectListItem> Organizations { get; set; }
    public int OrganizationId { get; set; }

    public ValidationResult? ValidationResult { get; set; }
  }
}