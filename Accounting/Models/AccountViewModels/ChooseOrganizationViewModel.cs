using Accounting.Models.OrganizationViewModels;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Accounting.Models.AccountViewModels
{
    public class ChooseOrganizationViewModel
    {
        public List<SelectListItem> Organizations { get; set; }
        public int OrganizationId { get; set; }

        public ValidationResult? ValidationResult { get; set; }
    }
}