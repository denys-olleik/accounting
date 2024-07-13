using Accounting.Common;
namespace Accounting.Business
{
  public class Address : IIdentifiable<int>
  {
    public int AddressID { get; set; }
    public string? ExtraAboveAddress { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? ExtraBelowAddress { get; set; }
    public string? City { get; set; }
    public string? StateProvince { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public int BusinessEntityId { get; set; }
    public int OrganizationId { get; set; }
    public int CreatedById { get; set; }

    public int Identifiable => this.AddressID;
  }
}