using Accounting.Common;

namespace Accounting.Business
{
  public class User : IIdentifiable<int>
  {
    public int UserID { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Password { get; set; } = string.Empty;
    public int CreatedById { get; set; }
    public DateTime Created { get; set; }

    public int Identifiable => UserID;

    public List<Organization>? Organizations { get; set; }
  }
}