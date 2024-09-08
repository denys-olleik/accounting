using FluentValidation.Results;

namespace Accounting.Models.SecretViewModels
{
  public class CreateSecretViewModel
  {
    public string? Key { get; set; }
    public string? Value { get; set; }
    public string? Vendor { get; set; }
    public string? Purpose { get; set; }
    public bool EncryptValue { get; set; }

    public ValidationResult? ValidationResult { get; set; }
  }
}