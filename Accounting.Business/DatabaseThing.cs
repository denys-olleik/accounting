using Accounting.Common;
using System.Reflection;

namespace Accounting.Business
{
  public class DatabaseThing : IIdentifiable<string>
  {
    public string? Name { get; set; }
    public string? Owner { get; set; }
    public string? Encoding { get; set; }
    public string? Collation { get; set; }
    public string? Ctype { get; set; }
    public int ConnectionLimit { get; set; }

    public string Identifiable => this.Name!;

    public static class DatabaseConstants
    {
      public const string DatabaseName = "Accounting";
      public const string DatabaseNameAdmin = "postgres";

      private static readonly List<string> _all = new List<string>();

      static DatabaseConstants()
      {
        var fields = typeof(DatabaseConstants).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
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