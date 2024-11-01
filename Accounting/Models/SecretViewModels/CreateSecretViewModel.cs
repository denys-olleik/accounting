using Accounting.Service;
using FluentValidation;
using FluentValidation.Results;

namespace Accounting.Models.SecretViewModels
{
  public class CreateSecretViewModel
  {
    private string? _key;
    private string? _value;
    private string? _type;
    private string? _purpose;

    public string? Key { get => _key; set => _key = value?.Trim(); }
    public string? Value { get => _value; set => _value = value?.Trim(); }
    public string? Type { get => _type; set => _type = value?.Trim(); }
    public string? Purpose { get => _purpose; set => _purpose = value?.Trim(); }

    public bool EncryptValue { get; set; }
    public bool Master { get; set; }
    public ValidationResult? ValidationResult { get; set; }
    public int OrganizationId { get; set; }
  }

  public class CreateSecretViewModelValidator : AbstractValidator<CreateSecretViewModel>
  {
    private readonly SecretService _secretService;

    public CreateSecretViewModelValidator(SecretService secretService)
    {
      _secretService = secretService;

      RuleFor(x => x.Value).NotEmpty().WithMessage("Value is required.");

      RuleFor(x => x.Type)
          .NotEmpty().When(x => !x.Master)
          .WithMessage("Type is required for non-master keys.")
          .MaximumLength(20).WithMessage("Type cannot exceed 20 characters.");

      RuleFor(x => x.Type)
          .Empty().When(x => x.Master)
          .WithMessage("Master keys cannot have a type.");

      RuleFor(x => x.Purpose).MaximumLength(100).WithMessage("Purpose cannot exceed 100 characters.");

      RuleFor(x => x.OrganizationId)
          .GreaterThan(0)
          .WithMessage("Request was missing organization id.");

      RuleFor(x => x)
        .MustAsync(KeyNotExists)
        .WithMessage("The key already exists.");

      RuleFor(x => x)
        .Must(x => !(x.Master && x.EncryptValue))
        .WithMessage("A master key cannot have its value encrypted.");

      RuleFor(x => x)
        .MustAsync(async (model, cancellationToken) => await MasterKeyExistsIfEncryptValueAsync(model))
        .WithMessage("A master key must be present if the value is to be encrypted.");
    }

    private async Task<bool> KeyNotExists(CreateSecretViewModel model, CancellationToken cancellationToken)
    {
      if (string.IsNullOrEmpty(model.Key))
        return true;

      var secret = await _secretService.GetAsync(model.Key, model.OrganizationId);
      return secret == null;
    }

    private async Task<bool> MasterKeyExistsIfEncryptValueAsync(CreateSecretViewModel model)
    {
      if (!model.EncryptValue)
        return true;

      var masterSecret = await _secretService.GetMasterAsync(model.OrganizationId);
      return masterSecret != null;
    }
  }
}