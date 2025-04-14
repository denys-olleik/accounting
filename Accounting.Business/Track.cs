using Accounting.Common;

namespace Accounting.Business
{
  public class Track : IIdentifiable<int>
  {
    public int TrackID { get; set; }
    public string? SpotifyTrackId { get; set; }
    public string? Title { get; set; }
    public string? Artist { get; set; }
    public string? Album { get; set; }
    public Guid Transaction { get; set; }

    public DateTime Created { get; set; }

    public int Identifiable => this.TrackID;
  }
}