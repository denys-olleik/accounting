using Accounting.Common;

namespace Accounting.Business
{
  public class DatabaseThing : IIdentifiable<int>
  {
    public int DatabaseID { get; set; }
    public string? Name { get; set; }
    public string? Owner { get; set; }
    public string? Encoding { get; set; }
    public string? Collation { get; set; }
    public string? Ctype { get; set; }
    public int ConnectionLimit { get; set; }

    public int Identifiable => this.DatabaseID;
  }
}