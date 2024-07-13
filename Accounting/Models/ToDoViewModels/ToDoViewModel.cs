namespace Accounting.Models.ToDoViewModels
{
  public class ToDoViewModel
  {
    public int ToDoID { get; set; }
    public string? Title { get; set; }
    public string? HtmlContent { get; set; }
    public string? MarkdownContent { get; set; }
    public int? ParentToDoId { get; set; }
    public List<ToDoViewModel>? Children { get; set; }
    public DateTime Created { get; set; }
  }
}