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
    //public string? ConnectionStringDefaultPsql { get; set; }
    //public string? ConnectionStringAdminPsql { get; set; }
    public bool TenantManagement { get; set; }


    private ConfigurationSingleton()
    {

    }
  }
}