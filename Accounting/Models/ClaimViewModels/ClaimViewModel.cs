namespace Accounting.Models.ClaimViewModels
{
    public class ClaimViewModel
    {
        public int ID { get; set; }
        public int UserId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue  { get; set; }
        public DateTime Created { get; set; }
    }
}