namespace Accounting.Models.ReconciliationViewModels
{
    public class ReconciliationDetailsViewModel : PaginatedViewModel
    {
        public int ReconciliationId { get; set; }
        public string? Status { get; set; }
        public string? OriginalFileName { get; set; }
        public int CreatedById { get; set; }
        public int OrganizationId { get; set; }
        public DateTime Created { get; set; }
    }
}