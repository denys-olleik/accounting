namespace Accounting.Business
{
  public class BoardCell
  {
    public int X { get; set; }
    public int Y { get; set; }
    public string Country { get; set; }
    public int FriendlyPlayerCount { get; set; }
    public bool OccupiedByEnemy { get; set; }
  }

  public class BoardState
  {
    public List<BoardCell> State { get; set; } = new();
  }
}