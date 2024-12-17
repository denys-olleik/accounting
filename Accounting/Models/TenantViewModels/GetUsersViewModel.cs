namespace Accounting.Models.TenantViewModels
{
  public class GetUsersViewModel : PaginatedViewModel
  {
    public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();

    public class UserViewModel
    {
      public int UserID { get; set; }
      public string Email { get; set; }
      public List<OrganizationViewModel> Organizations { get; set; } = new List<OrganizationViewModel>();
    }

    public class OrganizationViewModel
    {
      public int OrganizationID { get; set; }
      public string Name { get; set; }
    }
  }
}