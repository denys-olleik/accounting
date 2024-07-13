namespace Accounting.Models.ChartOfAccount
{
  public class ChartOfAccountViewModel
  {
    public ChartOfAccountViewModel()
    {
      Name = string.Empty;
      Type = string.Empty;
    }

    public int ChartOfAccountID { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public bool InvoiceCreationForCredit { get; set; }
    public bool InvoiceCreationForDebit { get; set; }
    public bool ReceiptOfPaymentForCredit { get; set; }
    public bool ReceiptOfPaymentForDebit { get; set; }
    public bool ReconciliationExpense { get; set; }
    public bool ReconciliationLiabilitiesAndAssets { get; set; }
    public DateTime Created { get; set; }
    public int? ParentChartOfAccountId { get; set; }
    public int CreatedById { get; set; }
    public List<ChartOfAccountViewModel>? Children { get; set; }
  }
}