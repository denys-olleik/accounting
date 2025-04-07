
namespace Accounting.Models.HomeViewModels
{
  public class LatestPostViewModel
  {
    public string BlogHtmlSanitizedContent { get; set; }
    public string Title { get; set; }
    public DateTime? Created { get; set; }
  }
}