namespace Accounting.Models.AddressViewModels
{
  public class AddressViewModel
  {
    public string? ID { get; set; }
    public string? ExtraAboveAddress { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? ExtraBelowAddress { get; set; }
    public string? City { get; set; }
    public string? StateProvince { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
  }
}