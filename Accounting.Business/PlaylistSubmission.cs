using Accounting.Common;

namespace Accounting.Business
{
  public class PlaylistSubmission : IIdentifiable<int>
  {
    public int PlaylistSubmissionID { get; set; }
    public int PlaylistLoverID { get; set; }
    public DateTime Created { get; set; }
    public int Identifiable => this.PlaylistSubmissionID;
  }
}