namespace Accounting.Models.ToDoViewModels
{
    public class UpdateToDoStatusBindingModel
    {
        public int ToDoId { get; set; }
        public string? Status { get; set; }
    }
}