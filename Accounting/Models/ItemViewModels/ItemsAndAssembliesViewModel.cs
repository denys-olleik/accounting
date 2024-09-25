namespace Accounting.Models.ItemViewModels
{
  public class ItemsAndAssembliesViewModel : PaginatedViewModel
  {
    public List<ItemViewModel>? Items { get; set; }

    public class ItemViewModel
    {
      public int ItemID { get; set; }
      public string? Name { get; set; }
      public string? Description { get; set; }
      public decimal Quantity { get; set; }
    }
  }
}