using Accounting.Common;

namespace Accounting.Business
{
  public class DatabaseThing : IIdentifiable<int>
  {
    public int DatabaseID { get; set; }

    public int Identifiable => this.DatabaseID;
  }
}