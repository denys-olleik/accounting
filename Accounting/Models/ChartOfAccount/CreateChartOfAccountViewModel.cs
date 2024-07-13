using FluentValidation.Results;

namespace Accounting.Models.ChartOfAccount
{
  public class CreateChartOfAccountViewModel
  {
    public int? ParentChartOfAccountId { get; set; }
    public ChartOfAccountViewModel? ParentChartOfAccount { get; set; }
    public string? AccountName { get; set; }
    public string? SelectedAccountType { get; set; }

    public bool ShowInInvoiceCreationDropDownForCredit { get; set; }
    public bool ShowInInvoiceCreationDropDownForDebit { get; set; }
    public bool ShowInReceiptOfPaymentDropDownForCredit { get; set; }
    public bool ShowInReceiptOfPaymentDropDownForDebit { get; set; }
    public bool ReconciliationExpense { get; set; }
    public bool ReconciliationLiabilitiesAndAssets { get; set; }


    public List<string> AvailableAccountTypes { get; set; } = new List<string>();

    public ValidationResult? ValidationResult { get; set; }

    public class ChartOfAccountViewModel
    {
      public int ChartOfAccountID { get; set; }
      public string? Name { get; set; }
    }
  }
}