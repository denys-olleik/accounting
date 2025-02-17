using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.ToDoViewModels;
using Accounting.Service;
using Markdig;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [ApiController]
  [Route("api/t")]
  public class ToDoApiController : BaseController
  {
    private readonly ToDoService _toDoService;
    private readonly UserTaskService _userTaskService;

    public ToDoApiController(RequestContext requestContext, ToDoService toDoService, UserTaskService userTaskService)
    {
      _toDoService = new ToDoService(requestContext.DatabaseName, requestContext.DatabasePassword);
      _userTaskService = new UserTaskService(requestContext.DatabaseName, requestContext.DatabasePassword);
    }

    [HttpGet("get-todos")]
    public async Task<IActionResult> GetTasks()
    {
      List<ToDo> toDos = await _toDoService.GetAllAsync(GetOrganizationId());

      foreach (var task in toDos)
      {
        await LoadUsersForTaskAndSubtasks(task, _userTaskService, GetOrganizationId());
      }

      var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
      foreach (var taskItem in toDos)
      {
        ConvertContentToHtml(taskItem, pipeline);
      }

      var responseModel = new ToDoItemsApiModel
      {
        ToDos = toDos.Select(task => MapToDoToViewModel(task)).ToList()
      };

      return Ok(responseModel);
    }

    [HttpPost("update-content")]
    public async Task<IActionResult> UpdateContent([FromBody] UpdateContentModel model)
    {
      ToDo task = await _toDoService.UpdateContentAsync(model.ToDoId, model.Content, GetOrganizationId());

      var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

      ToDoViewModel updatedTaskViewModel = new ToDoViewModel()
      {
        ToDoID = task.ToDoID,
        Title = task.Title,
        HtmlContent = task.Content != null ? Markdown.ToHtml(task.Content, pipeline) : string.Empty,
        MarkdownContent = task.Content,
        ParentToDoId = task.ParentToDoId,
        Children = new List<ToDoViewModel>(),
        Created = task.Created
      };

      return Ok(updatedTaskViewModel);
    }

    [HttpPost("update-todo-parent")]
    public async Task<IActionResult> UpdateTaskParent(UpdateTaskParentBindingModel model)
    {
      try
      {
        int rowsAffected = await _toDoService.UpdateParentTaskIdAsync(model.ToDoId, model.NewParentToDoId, GetOrganizationId());

        if (rowsAffected == 0)
        {
          return BadRequest(new { error = "The parent task was not updated. Check your input and try again." });
        }
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { error = ex.Message });
      }

      // If we reach this point, the parent task was updated successfully.
      return Ok();
    }

    [Route("get-todo-children/{toDoID}")]
    [HttpGet]
    public async Task<IActionResult> GetTaskChildren(int toDoID)
    {
      List<ToDo> children = await _toDoService.GetTaskChildren(toDoID, GetOrganizationId());

      foreach (var task in children)
      {
        await LoadUsersForTaskAndSubtasks(task, _userTaskService, GetOrganizationId());
      }

      var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
      foreach (var taskItem in children)
      {
        ConvertContentToHtml(taskItem, pipeline);
      }

      return Ok(children);
    }

    [Route("update-todo-status")]
    [HttpPost]
    public async Task<IActionResult> UpdateTaskStatusId(UpdateToDoStatusBindingModel model)
    {
      try
      {
        int rowsAffected = await _toDoService.UpdateTaskStatusIdAsync(model.ToDoId, model.Status!, GetOrganizationId());

        if (rowsAffected == 0)
        {
          return BadRequest(new { error = "The task status was not updated. Check your input and try again." });
        }
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { error = ex.Message });
      }

      return Ok();
    }

    private void ConvertContentToHtml(ToDo taskItem, MarkdownPipeline pipeline)
    {
      if (!string.IsNullOrWhiteSpace(taskItem.Content))
      {
        taskItem.Content = Markdown.ToHtml(taskItem.Content, pipeline);
      }

      foreach (var childTask in taskItem.Children)
      {
        ConvertContentToHtml(childTask, pipeline);
      }
    }

    private ToDoItemsApiModel.ToDoViewModel MapToDoToViewModel(ToDo task)
    {
      var viewModel = new ToDoItemsApiModel.ToDoViewModel
      {
        ToDoID = task.ToDoID,
        Title = task.Title,
        Content = task.Content,
        ParentToDoId = task.ParentToDoId,
        Status = task.Status,
        CreatedById = task.CreatedById,
        OrganizationId = task.OrganizationId,
        Created = task.Created,
        Children = new List<ToDoItemsApiModel.ToDoViewModel>(),
        Users = task.Users?.Select(u => new ToDoItemsApiModel.UserViewModel
        {
          UserID = u.UserID,
          FirstName = u.FirstName,
          LastName = u.LastName,
          Email = u.Email
        }).ToList()
      };

      foreach (var child in task.Children)
      {
        viewModel.Children.Add(MapToDoToViewModel(child));
      }

      return viewModel;
    }

    private async Task LoadUsersForTaskAndSubtasks(ToDo task, UserTaskService userTaskService, int organizationId)
    {
      task.Users = await userTaskService.GetUsers(task.ToDoID, organizationId);

      foreach (var subtask in task.Children)
      {
        await LoadUsersForTaskAndSubtasks(subtask, userTaskService, organizationId);
      }
    }
  }
}