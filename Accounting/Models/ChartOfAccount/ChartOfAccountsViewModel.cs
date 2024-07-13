namespace Accounting.Models.ChartOfAccount
{
  public class ChartOfAccountsViewModel
  {
    public List<ChartOfAccountViewModel> Accounts { get; set; }
      = new List<ChartOfAccountViewModel>();

    public class ChartOfAccountViewModel
    {
      public int ChartOfAccountID { get; set; }
      public string? Name { get; set; }
      public string? Type { get; set; }
      public int? ParentChartOfAccountId { get; set; }
    }
  }
}