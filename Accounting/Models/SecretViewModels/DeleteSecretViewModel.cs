namespace Accounting.Models.SecretViewModels
{
  public class DeleteSecretViewModel
  {
    public int SecretID { get; set; }
    public string? Key { get; set; }
    public string? Vendor { get; set; }
    public string? Purpose { get; set; }
  }
}