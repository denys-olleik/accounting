namespace Accounting.Models.SecretViewModels
{
  public class SecretsViewModel
  {
    public List<SecretViewModel>? Secrets { get; set; }

    public class SecretViewModel
    {
      public int SecretID { get; set; }
      public string? Key { get; set; }
      public bool Master { get; set; }
      public string? Vendor { get; set; }
      public string? Purpose { get; set; }
    }
  }
}