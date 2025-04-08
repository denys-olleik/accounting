using Accounting.Common;
using System.Reflection;

namespace Accounting.Business
{
  public class Secret : IIdentifiable<int>
  {
    public int SecretID { get; set; }
    public bool Master { get; set; }
    public string? Value { get; set; }
    public bool ValueEncrypted { get; set; }
    public string? Type { get; set; }
    public string? Purpose { get; set; }
    public DateTime Created { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }

    public User? CreatedBy { get; set; }
    public Organization? Organization { get; set; }

    public int Identifiable => this.SecretID;

    public static class SecretTypeConstants
    {
      public const string Email = "email";
      public const string SMS = "sms";
      public const string Cloud = "cloud";
      public const string NoReply = "no-reply";
      public const string DropletLimit = "droplet-limit";

      private static readonly List<string> _all = new List<string>();

      static SecretTypeConstants()
      {
        var fields = typeof(SecretTypeConstants).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
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