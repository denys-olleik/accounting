namespace Accounting.Models
{
    public abstract class PaginatedViewModel
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int? NextPage { get; set; }
    }
}