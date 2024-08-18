using Accounting.Common;

namespace Accounting.Business
{
  public class ZipCode : IIdentifiable<int>
  {
    public int ID { get; set; }
    public string? Zip5 { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
    public string? City { get; set; }
    public string? State2 { get; set; }
    public int Identifiable => this.ID;
  }
}