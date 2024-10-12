using Accounting.Common;

namespace Accounting.Business
{
  public class Tenant : IIdentifiable<int>
  {
    public int TenantID { get; set; }
    public string? FullyQualifiedDomainName { get; set; }
    public string? Email { get; set; }
    public long? DropletId { get; set; }
    public string? Ipv4 { get; set; }
    public string? SshPublic { get; set; }
    public string? SshPrivate { get; set; }
    public int? CreatedById { get; set; }
    public int? OrganizationId { get; set; }
    public DateTime Created { get; set; }

    public int Identifiable => this.TenantID;
  }
}