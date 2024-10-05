using Accounting.Models.AddressViewModels;

namespace Accounting.Models.BusinessEntityViewModels
{
  public class BusinessEntityViewModel
  {
    public int ID { get; set; }
    public string? CustomerType { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? CompanyName { get; set; }
    public string? Website { get; set; }
    public int PaymentTermId { get; set; }
    public int CreatedById { get; set; }
    public DateTime Created { get; set; }

    public List<AddressViewModel>? Addresses { get; set; }
    public int RowNumber { get; set; }
  }
}