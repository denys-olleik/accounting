namespace Accounting.Common
{
  public sealed class ConfigurationSingleton
  {
    private static readonly Lazy<ConfigurationSingleton> _lazy
        = new Lazy<ConfigurationSingleton>(() => new ConfigurationSingleton());

    public static ConfigurationSingleton Instance { get { return _lazy.Value; } }

    public string? ApplicationName { get; set; }
    public string? ConnectionString { get; set; }
    public int InvitationExpirationMinutes { get; set; }
    public string? AdminPsql { get; set; }
    public string? SendgridKey { get; set; }
    public string? NoReplyEmailAddress { get; set; }
    public string? AttachmentsPath { get; set; }
    public string? TempPath { get; set; }
    public string? PermPath { get; set; }
    public string? ConnectionStringPsql { get; set; }

    private ConfigurationSingleton()
    {

    }
  }
}