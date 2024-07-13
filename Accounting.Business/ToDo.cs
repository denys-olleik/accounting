using Accounting.Common;
using System.Reflection;

namespace Accounting.Business
{
  public class ToDo : IIdentifiable<int>
  {
    public int ToDoID { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public int? ParentToDoId { get; set; }
    public string? Status { get; set; }
    public int CreatedById { get; set; }
    public User? CreatedBy { get; set; }
    public int OrganizationId { get; set; }
    public DateTime Created { get; set; }

    #region Extra properties.
    public List<ToDo> Children { get; set; } = new List<ToDo>();
    public List<User>? Users { get; set; }
    #endregion

    public int Identifiable => ToDoID;

    public static class ToDoStatuses
    {
      public const string Open = "open";
      public const string Closed = "closed";
      public const string Completed = "completed";

      private static readonly List<string> _all = new List<string>();

      static ToDoStatuses()
      {
        var fields = typeof(ToDoStatuses).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
        foreach (var field in fields)
        {
          if (field.FieldType == typeof(string) && field.GetValue(null) is string value)
          {
            _all.Add(value);
          }
        }
      }

      public static IReadOnlyList<string> All => _all.AsReadOnly();
    }
  }
}