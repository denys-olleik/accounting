namespace Accounting.Models.SecretViewModels
{
  public class DeleteSecretViewModel
  {
    public int SecretID { get; set; }
    public string? Type { get; set; }
    public string? Purpose { get; set; }
  }
}