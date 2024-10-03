using Accounting.Common;
using System.Reflection;

namespace Accounting.Business
{
  public class Invoice : IIdentifiable<int>
  {
    public int InvoiceID { get; set; }
    public string? InvoiceNumber { get; set; }
    public int BusinessEntityId { get; set; }
    public string? BillingAddressJSON { get; set; }
    public Address? BillingAddress { get; set; }
    public string? ShippingAddressJSON { get; set; }
    public Address? ShippingAddress { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Status { get; set; }
    public string? PaymentInstructions { get; set; }
    public decimal TotalAmount { get; set; }
    public string? VoidReason { get; set; }
    public int CreatedById { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastUpdated { get; set; }
    public int OrganizationId { get; set; }

    public BusinessEntity? BusinessEntity { get; set; }
    public Organization? IssuingOrganization { get; set; }

    #region Extra properties
    public List<InvoiceLine>? InvoiceLines { get; set; }
    public List<Payment>? Payments { get; set; }
    public int? RowNumber { get; set; }
    public decimal? Received { get; set; }
    #endregion

    public int Identifiable => this.InvoiceID;

    public static class InvoiceStatusConstants
    {
      public const string Unpaid = "unpaid";
      public const string PartiallyPaid = "partially-paid";
      public const string Paid = "paid";
      public const string Void = "void";

      private static readonly List<string> _all = new List<string>();

      static InvoiceStatusConstants()
      {
        var fields = typeof(InvoiceStatusConstants).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
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