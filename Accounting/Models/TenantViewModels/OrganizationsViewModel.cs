namespace Accounting.Models.TenantViewModels
{
  public class OrganizationsViewModel
  {
    public List<OrganizationViewModel> Organizations { get; set; }
    public int TenantId { get; set; }

    public class OrganizationViewModel
    {
      public string Name { get; set; }
    }
  }
}