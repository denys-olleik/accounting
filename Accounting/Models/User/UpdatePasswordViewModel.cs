namespace Accounting.Models.User
{
  public class UpdatePasswordViewModel
  {
    public int UserId { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
  }
}