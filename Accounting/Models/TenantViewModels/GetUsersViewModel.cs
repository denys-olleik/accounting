namespace Accounting.Models.TenantViewModels
{
  public class GetUsersViewModel
  {
    public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();

    public class UserViewModel
    {
      public int UserID { get; set; }
      public string Email { get; set; }
      public OrganizationViewModel? Organization { get; set; }
    }

    public class OrganizationViewModel
    {
      public int OrganizationID { get; set; }
      public string Name { get; set; }
    }
  }
}