namespace Accounting.Models.BusinessEntityViewModels
{
    public class GetBusinessEntitiesViewModel
    {
        public List<BusinessEntityViewModel>? BusinessEntities { get; set; }
        public int CurrentPage { get; set; }
        public int? NextPage { get; set; }
    }
}