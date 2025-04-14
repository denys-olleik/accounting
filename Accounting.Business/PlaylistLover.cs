using Accounting.Common;

namespace Accounting.Business
{
  public class PlaylistLover : IIdentifiable<int>
  {
    public int PlaylistLoverID { get; set; }
    public string? Email { get; set; }
    public string? Code { get; set; }
    public DateTime CodeExpiration { get; set; }
    public bool Gender { get; set; }
    public DateTime Created { get; set; }

    public int Identifiable => this.PlaylistLoverID;
  }
}