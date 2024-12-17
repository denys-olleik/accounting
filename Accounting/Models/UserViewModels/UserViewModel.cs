using Accounting.Models.ClaimViewModels;

namespace Accounting.Models.UserViewModels
{
  public class UserViewModel
  {
    public int UserID { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }

    public List<ClaimViewModel>? Claims { get; set; }
  }
}