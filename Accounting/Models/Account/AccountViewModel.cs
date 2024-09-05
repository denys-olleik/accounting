namespace Accounting.Models.Account
{
  public class AccountViewModel
  {
    public AccountViewModel()
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
    public List<AccountViewModel>? Children { get; set; }
  }
}