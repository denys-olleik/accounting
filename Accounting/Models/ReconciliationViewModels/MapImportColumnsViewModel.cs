namespace Accounting.Models.ReconciliationViewModels
{
    public class MapImportColumnsViewModel
    {
        public List<string> ColumnNames { get; set; }
        public int ReconciliationAttachmentId { get; set; }
        public Dictionary<string, string> SelectedColumns { get; set; }
    }
}