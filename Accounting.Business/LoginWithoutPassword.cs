using Accounting.Common;

namespace Accounting.Business
{
  public class LoginWithoutPassword : IIdentifiable<int>
  {
    public int LoginWithoutPasswordID { get; set; }
    public string? Code { get; set; }
    public string? Email { get; set; }
    public DateTime Expires { get; set; }
    public DateTime? Completed { get; set; }
    public DateTime Created { get; set; }

    #region Computed properties
    public bool IsExpired => DateTime.UtcNow > Expires;
    #endregion

    public int Identifiable => this.LoginWithoutPasswordID;
  }
}