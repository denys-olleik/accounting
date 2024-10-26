using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Common
{
  public class PrefixConstants
  {
    public const string TenantDatabasePrefix = "tenant_";

    private static readonly List<string> _all = new List<string>();

    static PrefixConstants()
    {
      var fields = typeof(PrefixConstants).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
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