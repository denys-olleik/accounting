namespace Accounting.Models.TenantViewModels
{
  public class TenantViewModel
  {
    public int TenantID { get; set; }
    public string? DatabaseName { get; set; }
    public string? FullyQualifiedDomainName { get; set; }
    public string? Email { get; set; }
    public long? DropletId { get; set; }
    public string? Ipv4 { get; set; }
    public bool SshPublic { get; set; }
    public bool SshPrivate { get; set; }
    public DateTime Created { get; set; }

    #region Extra properties
    public int? RowNumber { get; set; }
    #endregion
  }
}