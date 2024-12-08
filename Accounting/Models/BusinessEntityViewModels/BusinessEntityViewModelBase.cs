using Accounting.Models.AddressViewModels;
using Accounting.Models.PaymentTermViewModels;
using FluentValidation.Results;

namespace Accounting.Models.BusinessEntityViewModels
{
  public class BusinessEntityViewModelBase
  {
    private string? _firstName;
    private string? _lastName;
    private string? _companyName;

    public string? FirstName
    {
      get => _firstName;
      set => _firstName = value?.Trim();
    }

    public string? LastName
    {
      get => _lastName;
      set => _lastName = value?.Trim();
    }

    public string? CompanyName
    {
      get => _companyName;
      set => _companyName = value?.Trim();
    }

    public List<string>? CustomerTypes { get; set; }

    public string? SelectedCustomerType { get; set; }

    public string? AvailableBusinessEntityTypesCsv { get; set; }
    public string? SelectedBusinessEntityTypesCsv { get; set; }

    public List<AddressViewModel>? Addresses { get; set; }
    public string? AddressesJson { get; set; }
    public List<PaymentTermViewModel>? AvailablePaymentTerms { get; set; }
    public PaymentTermViewModel? PaymentTerm { get; set; }
    public int SelectedPaymentTermId { get; set; }

    public ValidationResult? ValidationResult { get; set; }
  }
}