using Accounting.Common;

namespace Accounting.Business
{
  public class UserToDo : IIdentifiable<int>
  {
    public int UserToDoID { get; set; }
    public int UserId { get; set; }
    public int ToDoId { get; set; }
    public bool Completed { get; set; }
    public int OrganizationId { get; set; }
    public int CreatedById { get; set; }

    public int Identifiable => this.UserToDoID;
  }
}