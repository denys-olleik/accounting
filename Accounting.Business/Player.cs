using Accounting.Common;

namespace Accounting.Business
{
  public class Player : IIdentifiable<int>
  {
    public int PlayerID { get; set; }
    public Guid Guid { get; set; }
    public string? IpAddress { get; set; }
    public string? Country { get; set; }
    public int CurrentX { get; set; }
    public int CurrentY { get; set; }
    public int RequestedX { get; set; }
    public int RequestedY { get; set; }
    public DateTime? Updated { get; set; }
    public int Identifiable => this.PlayerID;

    public object AssignmentStatus { get; set; }
  }
}