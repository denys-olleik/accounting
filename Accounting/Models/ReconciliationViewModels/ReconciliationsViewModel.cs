namespace Accounting.Models.ReconciliationViewModels
{
  public class ReconciliationsViewModel
  {
    public List<ReconciliationViewModel>? Reconciliations { get; set; }

    public class ReconciliationViewModel
    {
      public int ID { get; set; }
      public string? Status { get; set; }
      public string? OriginalFileName { get; set; }
      public DateTime Created { get; set; }
      public int CreatedById { get; set; }
      public int OrganizationId { get; set; }
    }
  }
}