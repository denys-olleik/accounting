using System.Reflection;

namespace Accounting.Common
{
  public static class TypesToLoadConstants
  {
    public const string Invoice = "invoice";
    public const string Payment = "payment";

    private static readonly List<string> _all = new List<string>();

    static TypesToLoadConstants()
    {
      var fields = typeof(TypesToLoadConstants).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
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