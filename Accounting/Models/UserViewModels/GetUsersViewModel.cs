using Accounting.Common;

namespace Accounting.Models.UserViewModels
{
  public class GetUsersViewModel : PaginatedViewModel
  {
    public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();

    public class UserViewModel : IRowNumber
    {
      public int UserID { get; set; }
      public string? FirstName { get; set; }
      public string? LastName { get; set; }
      public string? Email { get; set; }
      public int? RowNumber { get; set; }
    }
  }
}