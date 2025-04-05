using Accounting.Common;

namespace Accounting.Business
{
  public class Blog : IIdentifiable<int>
  {
    public int BlogID { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }

    public int Identifiable => this.BlogID;
  }
}