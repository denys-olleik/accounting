namespace Accounting.Models.SecretViewModels
{
  public class SecretsViewModel
  {
    public List<SecretViewModel>? Secrets { get; set; }

    public class SecretViewModel
    {
      public int SecretID { get; set; }
      public bool Master { get; set; }
      public string? Type { get; set; }
      public string? Purpose { get; set; }
    }
  }
}