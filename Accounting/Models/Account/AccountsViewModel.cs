namespace Accounting.Models.Account
{
  public class AccountsViewModel
  {
    public List<AccountViewModel> Accounts { get; set; }
      = new List<AccountViewModel>();

    public class AccountViewModel
    {
      public int AccountID { get; set; }
      public string? Name { get; set; }
      public string? Type { get; set; }
      public int? ParentAccountId { get; set; }
    }
  }
}