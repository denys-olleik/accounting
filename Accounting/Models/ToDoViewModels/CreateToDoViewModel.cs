using Accounting.Models.TagViewModels;
using Accounting.Models.UserViewModels;
using FluentValidation.Results;

namespace Accounting.Models.ToDoViewModels
{
  public class CreateToDoViewModel
  {
    public string? Title { get; set; }
    public string? Content { get; set; }
    public List<UserViewModel>? Users { get; set; }
    public List<int>? SelectedUsers { get; set; }
    public string? SelectedToDoStatus { get; set; }
    public List<string>? ToDoStatuses { get; set; }
    public List<TagViewModel>? AvailableTags { get; set; }
    public List<TagViewModel>? SelectedTags { get; set; }
    public string? SelectedTagIds { get; set; }

    public ValidationResult? ValidationResult { get; set; }
    public int? ParentToDoId { get; set; }
    public ToDoViewModel? ParentToDo { get; set; }
  }
}