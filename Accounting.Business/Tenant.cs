using Accounting.Common;

namespace Accounting.Business
{
  public class Tenant : IIdentifiable<int>
  {
    public int TenantID { get; set; }
    public string? PublicId { get; set; }
    public string? DatabaseName { get; set; }
    public string? DatabasePassword { get; set; }
    public string? FullyQualifiedDomainName { get; set; }
    public string? Email { get; set; }
    public long? DropletId { get; set; }
    public string? Ipv4 { get; set; }
    public string? SshPublic { get; set; }
    public string? SshPrivate { get; set; }
    public string? HomepageMessage { get; set; }
    public int? CreatedById { get; set; }
    public DateTime Created { get; set; }

    #region Extra properties
    public int? RowNumber { get; set; }
    #endregion

    public int Identifiable => this.TenantID;
  }
}