namespace Accounting.Models.TenantViewModels
{
  public class GetUserOrganizationsViewModel
  {
    public List<UserOrganization>? UserOrganizations { get; set; }

    public class UserOrganization
    {
      public int UserOrganizationID { get; set; }
      public int UserID { get; set; }
      public UserViewModel? User { get; set; }
      public int OrganizationID { get; set; }
      public OrganizationViewModel? Organization { get; set; }
    }

    public class OrganizationViewModel
    {
      public int OrganizationID { get; set; }
      public string? Name { get; set; }
    }

    public class UserViewModel
    {
      public int UserID { get; set; }
      public string? Email { get; set; }
    }
  }
}