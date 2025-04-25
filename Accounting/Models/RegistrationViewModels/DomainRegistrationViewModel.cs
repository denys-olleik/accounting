namespace Accounting.Models.RegistrationViewModels
{
  public abstract class DomainRegistrationViewModel : BaseRegistrationViewModel
  {
    private string? _fullyQualifiedDomainName;

    public string? FullyQualifiedDomainName
    {
      get => _fullyQualifiedDomainName;
      set => _fullyQualifiedDomainName = value?.Trim();
    }
  }
}