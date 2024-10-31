using Accounting.Common;
using System.Reflection;

namespace Accounting.Business
{
  public class ApplicationSetting : IIdentifiable<int>
  {
    public int ApplicationSettingID { get; set; }
    public string? Key { get; set; }
    public string? Value { get; set; }
    public DateTime Created { get; set; }

    public int Identifiable => this.ApplicationSettingID;

    public class ApplicationSettingsConstants
    {
      public const string TenantManagement = "tenant-management";
      public const string NoReplyEmailAddress = "noreply";

      private static readonly List<string> _all = new List<string>();

      static ApplicationSettingsConstants()
      {
        var fields = typeof(ApplicationSettingsConstants)
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