namespace Accounting.Business
{
  public class UserReferenceInfo
  {
    public string? Schema { get; set; }
    public string? Table { get; set; }
    public string? Column { get; set; }
    public int ReferenceCount { get; set; }
  }
}