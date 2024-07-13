using Accounting.Common;

namespace Accounting.Business
{
  public class ToDoTag : IIdentifiable<int>
  {
    public int ToDoTagID { get; set; }
    public int TaskId { get; set; }
    public int TagId { get; set; }
    public int OrganizationId { get; set; }

    public int Identifiable => this.ToDoTagID;
  }
}