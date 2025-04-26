using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Common
{
  public static class UserRoleClaimConstants
  {
    public const string UserRole = "administrator";

    private static readonly List<string> _all = new List<string>();

    static UserRoleClaimConstants()
    {
      var fields = typeof(UserRoleClaimConstants).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
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