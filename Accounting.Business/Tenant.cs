using Accounting.Common;

namespace Accounting.Business
{
  public class Tenant : IIdentifiable<int>
  {
    public int TenantID { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Ipv4 { get; set; }
    public string? VmHostname { get; set; }
    public string? SHHPublic { get; set; }
    public int? CreatedById { get; set; }
    public DateTime Created { get; set; }

    public int Identifiable => this.TenantID;
  }
}