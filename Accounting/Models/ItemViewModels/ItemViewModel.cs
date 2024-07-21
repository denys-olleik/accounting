namespace Accounting.Models.ItemViewModels
{
  public class ItemViewModel
  {
    public int ID { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Price { get; set; }
    public int? RevenueChartOfAccountId { get; set; }
    public int? AssetsChartOfAccountId { get; set; }
  }
}