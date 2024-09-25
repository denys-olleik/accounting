using Accounting.Common;
using System.Reflection;

namespace Accounting.Business
{
  public class Item : IIdentifiable<int>
  {
    public int ItemID { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Quantity { get; set; }
    public int? UnitTypeId { get; set; }
    public string? ItemType { get; set; }
    public string InventoryMethod { get; set; } = InventoryMethods.FIFO;
    public int? RevenueAccountId { get; set; }
    public int? AssetsAccountId { get; set; }
    public int? ParentItemId { get; set; }
    public DateTime Created { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }

    public List<Inventory>? Inventories { get; set; }
    public List<Item>? Children { get; set; }

    public int Identifiable => this.ItemID;

    public int? RowNumber { get; set; }

    public static class InventoryMethods
    {
      public const string FIFO = "fifo";
      public const string LIFO = "lifo";
      public const string Any = "any";
      public const string Specific = "specific";

      private static readonly List<string> _all = new List<string>();

      static InventoryMethods()
      {
        var fields = typeof(InventoryMethods).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
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

    public static class ItemTypes
    {
      public const string Product = "product";
      public const string Service = "service";

      private static readonly List<string> _all = new List<string>();

      static ItemTypes()
      {
        var fields = typeof(ItemTypes).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
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