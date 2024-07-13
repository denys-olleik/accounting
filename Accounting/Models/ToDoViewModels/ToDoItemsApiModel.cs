namespace Accounting.Models.ToDoViewModels
{
  public class ToDoItemsApiModel
  {
    public List<ToDoViewModel>? ToDos { get; set; }

    public class ToDoViewModel
    {
      public int ToDoID { get; set; }
      public string? Title { get; set; }
      public string? Content { get; set; }

      public int? ParentToDoId { get; set; }
      public string? Status { get; set; }
      public int CreatedById { get; set; }
      public int OrganizationId { get; set; }
      public DateTime Created { get; set; }

      #region Extra properties.
      public List<ToDoViewModel>? Children { get; set; }
      public List<UserViewModel>? Users { get; set; }
      #endregion
    }

    public class UserViewModel
    {
      public int UserID { get; set; }
      public string? FirstName { get; set; }
      public string? LastName { get; set; }
      public string? Email { get; set; }
    }
  }
}