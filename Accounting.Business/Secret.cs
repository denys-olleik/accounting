using Accounting.Common;
using System.Reflection;

namespace Accounting.Business
{
  public class Secret : IIdentifiable<int>
  {
    public int SecretID { get; set; }
    public string? Key { get; set; }
    public string? Value { get; set; }
    public string? Vendor { get; set; }
    public string? Purpose { get; set; }
    public DateTime Created { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }

    public User? CreatedBy { get; set; }
    public Organization? Organization { get; set; }

    public int Identifiable => this.SecretID;

    public static class SecretVendorConstants
    {
      public const string DigitalOcean = "digital-ocean";
      public const string SendGrid = "sendgrid";

      private static readonly List<string> _all = new List<string>();

      static SecretVendorConstants()
      {
        var fields = typeof(SecretVendorConstants).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
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