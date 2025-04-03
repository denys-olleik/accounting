using System.Reflection;

namespace Accounting.Common
{
  public sealed class ConfigurationSingleton
  {
    private static readonly Lazy<ConfigurationSingleton> _lazy
        = new Lazy<ConfigurationSingleton>(() => new ConfigurationSingleton());

    public static ConfigurationSingleton Instance { get { return _lazy.Value; } }

    public string? ApplicationName { get; set; }
    public string DigitalOceanKey { get; set; }
    public string? TempPath { get; set; }
    public string? PermPath { get; set; }
    public string DatabaseName { get; set; }
    public string DatabasePassword { get; set; }
    public bool TenantManagement { get; set; }


    private ConfigurationSingleton()
    {

    }

    public static class ConfigurationConstants
    {
      public const string TenantManagement = "tenant-management";

      private static readonly List<string> _all = new List<string>();

      static ConfigurationConstants()
      {
        var fields = typeof(ConfigurationConstants).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
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