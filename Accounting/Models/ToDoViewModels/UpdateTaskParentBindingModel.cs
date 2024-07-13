namespace Accounting.Models.ToDoViewModels
{
  public class UpdateTaskParentBindingModel
  {
    public int ToDoId { get; set; }
    public int? NewParentToDoId { get; set; }
  }
}