using Accounting.Common;
using System.Reflection;

namespace Accounting.Business
{
  public class Reconciliation : IIdentifiable<int>
  {
    public int ReconciliationID { get; set; }
    public string? Status { get; set; }
    public DateTime Created { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }

    public int Identifiable => this.ReconciliationID;

    public ReconciliationAttachment? ReconciliationAttachment { get; set; }

    public static class Statuses
    {
      public const string Pending = "pending";
      public const string Processed = "processed";

      private static readonly List<string> _all = new List<string>();

      static Statuses()
      {
        var fields = typeof(Statuses)
            .GetFields(
            BindingFlags.Public
            | BindingFlags.Static
            | BindingFlags.DeclaredOnly);

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