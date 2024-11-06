﻿using FluentValidation.Results;

namespace Accounting.Models.UserAccountViewModels
{
  public class ChooseOrganizationViewModel
  {
    public List<OrganizationViewModel> Organizations { get; set; } = null!;
    public int? SelectedOrganizationId { get; set; }
    public string? SelectedPublicTenantId { get; set; }
    public ValidationResult? ValidationResult { get; set; }
  }

  public class OrganizationViewModel
  {
    public int OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? TenantPublicId { get; set; }
  }
}