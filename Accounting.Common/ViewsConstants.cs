using System.Reflection;

namespace Accounting.Common
{
  public class ViewsConstants
  {
    public const string CreateProductOrService = "create-product-or-service";
    public const string ReceivePayment = "receive-payment";

    private static readonly List<string> _all = new List<string>();

    static ViewsConstants()
    {
      var fields = typeof(ViewsConstants).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
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