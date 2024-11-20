namespace Accounting.Models.TenantViewModels
{
  public class UpdateUserViewModel
  {
    public int TenantId { get; set; }
    public int UserID { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<OrganizationViewModel> AvailableOrganizations { get; set; } = new List<OrganizationViewModel>();
    public string SelectedOrganizationIdsCsv { get; set; }



    public class OrganizationViewModel
    {
      public int OrganizationID { get; set; }
      public string Name { get; set; }
    }
  }
}