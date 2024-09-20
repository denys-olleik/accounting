using Accounting.Common;
using System.Reflection;

namespace Accounting.Business
{
  public class Account : IIdentifiable<int>
  {
    public Account()
    {
      Name = string.Empty;
      Type = string.Empty;
    }

    public int AccountID { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public bool InvoiceCreationForCredit { get; set; }
    public bool InvoiceCreationForDebit { get; set; }
    public bool ReceiptOfPaymentForCredit { get; set; }
    public bool ReceiptOfPaymentForDebit { get; set; }
    public bool ReconciliationExpense { get; set; }
    public bool ReconciliationLiabilitiesAndAssets { get; set; }
    public DateTime Created { get; set; }
    public int? ParentAccountId { get; set; }
    public int CreatedById { get; set; }
    public int OrganizationId { get; set; }

    public decimal CurrentBalance { get; set; }
    public int JournalEntryCount { get; set; }
    public List<Account>? Children { get; set; }

    public int Identifiable => this.AccountID;

    public static class AccountTypeConstants
    {
      public const string Assets = "assets";
      public const string Liabilities = "liabilities";
      public const string Equity = "equity";
      public const string Revenue = "revenue";
      public const string Expense = "expense";

      private static readonly List<string> _all = new List<string>();

      static AccountTypeConstants()
      {
        var fields = typeof(AccountTypeConstants).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
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