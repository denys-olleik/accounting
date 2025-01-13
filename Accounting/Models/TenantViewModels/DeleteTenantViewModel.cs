namespace Accounting.Models.TenantViewModels
{
  public class DeleteTenantViewModel
  {
    public int TenantId { get; set; }
    public bool DeleteDatabase { get; set; }
    public string? DatabaseName { get; set; }
  }
}