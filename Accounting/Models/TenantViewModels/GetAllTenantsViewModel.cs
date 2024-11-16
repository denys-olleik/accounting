namespace Accounting.Models.TenantViewModels
{
  public class GetAllTenantsViewModel : PaginatedViewModel
  {
    public List<TenantViewModel>? Tenants { get; set; }
  }
}