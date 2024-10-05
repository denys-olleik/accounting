using Accounting.Common;
using System.Reflection;
namespace Accounting.Business
{
  public class BusinessEntity : IIdentifiable<int>
  {
    public int BusinessEntityID { get; set; }
    public string? CustomerType { get; set; }
    public string? BusinessEntityTypesCsv { get; set; }
    public List<string>? BusinessEntityTypes
    {
      get { return BusinessEntityTypesCsv?.Split(',')?.ToList() ?? new List<string>(); }
    }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? CompanyName { get; set; }
    public string? Website { get; set; }
    public int PaymentTermId { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }
    public DateTime Created { get; set; }
    public int Identifiable => this.BusinessEntityID;

    public List<Address>? Addresses { get; set; }
    public PaymentTerm? PaymentTerm { get; set; }
    public int RowNumber { get; set; }

    public static class CustomerTypeConstants
    {
      public const string Individual = "individual";
      public const string Company = "company";

      private static readonly List<string> _all = new List<string>();

      static CustomerTypeConstants()
      {
        var fields = typeof(CustomerTypeConstants).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
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

    public static class BusinessEntityTypeConstants
    {
      public const string Customer = "customer";
      public const string Vendor = "vendor";

      private static readonly List<string> _all = new List<string>();

      static BusinessEntityTypeConstants()
      {
        var fields = typeof(BusinessEntityTypeConstants).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
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