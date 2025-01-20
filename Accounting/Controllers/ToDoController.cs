using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.TagViewModels;
using Accounting.Models.ToDoViewModels;
using Accounting.Models.UserViewModels;
using Accounting.Service;
using Accounting.Validators;
using FluentValidation.Results;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Transactions;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("t")]
  public class ToDoController : BaseController
  {
    private readonly TagService _tagService;
    private readonly UserService _userService;
    private readonly UserTaskService _userTaskService;
    private readonly ToDoService _toDoService;
    private readonly ToDoTagService _toDoTagService;

    public ToDoController(
      RequestContext requestContext, 
      TagService tagService, 
      UserService userService, 
      UserTaskService userTaskService, 
      ToDoService toDoService, 
      ToDoTagService toDoTagService)
    {
      _tagService = new TagService(requestContext.DatabasePassword, requestContext.DatabaseName);
      _userService = new UserService(requestContext.DatabasePassword, requestContext.DatabaseName);
      _userTaskService = new UserTaskService(requestContext.DatabasePassword, requestContext.DatabaseName);
      _toDoService = new ToDoService(requestContext.DatabasePassword, requestContext.DatabaseName);
      _toDoTagService = new ToDoTagService(requestContext.DatabasePassword, requestContext.DatabaseName);
    }

    [Route("todos")]
    [HttpGet]
    public async Task<IActionResult> ToDos()
    {
      ToDosPaginatedViewModel vm = new ToDosPaginatedViewModel();
      vm.AvailableStatuses = ToDo.ToDoStatuses.All.ToList();

      return View(vm);
    }

    [HttpGet]
    [Route("create")]
    public async Task<IActionResult> Create(int? parentToDoId)
    {
      List<Tag> tags = await _tagService.GetAllAsync();

      CreateToDoViewModel createToDoViewModel = new CreateToDoViewModel();
      createToDoViewModel.Users = (await _userService.GetAllAsync(GetOrganizationId())).Select(user => new UserViewModel
      {
        UserID = user.UserID,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName
      }).ToList();

      createToDoViewModel.AvailableTags = tags.Select(tag => new TagViewModel
      {
        ID = tag.TagID,
        Name = tag.Name
      }).ToList();

      createToDoViewModel.ParentToDoId = parentToDoId;

      ToDo? toDo = parentToDoId.HasValue ? await _toDoService.GetAsync(parentToDoId.Value, GetOrganizationId()) : null;
      createToDoViewModel.ParentToDo = toDo != null ? new ToDoViewModel
      {
        ToDoID = toDo.ToDoID,
        Title = toDo.Title,
        HtmlContent = toDo.Content
      } : null;

      createToDoViewModel.ToDoStatuses = ToDo.ToDoStatuses.All.Select(s => s.ToLower()).ToList();

      return View(createToDoViewModel);
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create(CreateToDoViewModel model)
    {
      CreateTaskViewModelValidator validator = new CreateTaskViewModelValidator();
      ValidationResult validationResult = await validator.ValidateAsync(model);

      var deserializedSelectedTagIds = JsonConvert.DeserializeObject<List<int>>(model.SelectedTagIds!);

      ToDo? parentToDoItem = model.ParentToDoId.HasValue ? await _toDoService.GetAsync(model.ParentToDoId.Value, GetOrganizationId()) : null;
      model.ParentToDo = parentToDoItem != null ? new ToDoViewModel
      {
        ToDoID = parentToDoItem.ToDoID,
        Title = parentToDoItem.Title,
        HtmlContent = parentToDoItem.Content
      } : null;

      if (!validationResult.IsValid)
      {
        List<Tag> tags = await _tagService.GetAllAsync();

        if (deserializedSelectedTagIds != null && deserializedSelectedTagIds.Any())
        {
          model.SelectedTags = tags.Where(tag => deserializedSelectedTagIds.Contains(tag.TagID))
              .Select(tag => new TagViewModel
              {
                ID = tag.TagID,
                Name = tag.Name
              }).ToList();

          tags = tags.Where(tag => !deserializedSelectedTagIds.Contains(tag.TagID)).ToList();
        }

        model.ParentToDoId = model.ParentToDoId;
        model.ToDoStatuses = ToDo.ToDoStatuses.All.Select(s => s.ToLower()).ToList();

        model.Users = (await _userService.GetAllAsync(GetOrganizationId())).Select(user => new UserViewModel
        {
          UserID = user.UserID,
          Email = user.Email,
          FirstName = user.FirstName,
          LastName = user.LastName
        }).ToList();

        model.AvailableTags = tags.Select(tag => new TagViewModel
        {
          ID = tag.TagID,
          Name = tag.Name
        }).ToList();

        model.ValidationResult = validationResult;
        return View(model);
      }

      using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        ToDo taskItem = await _toDoService.CreateAsync(new ToDo()
        {
          Title = model.Title,
          Content = model.Content,
          ParentToDoId = model.ParentToDoId,
          Status = model.SelectedToDoStatus,
          CreatedById = GetUserId(),
          OrganizationId = GetOrganizationId()
        });

        if (model.SelectedUsers != null && model.SelectedUsers.Any())
        {
          foreach (int userId in model.SelectedUsers)
          {
            UserToDo userTask = new UserToDo();
            userTask.UserId = userId;
            userTask.ToDoId = taskItem.ToDoID;
            userTask.Completed = false;
            userTask.OrganizationId = GetOrganizationId();
            userTask.CreatedById = GetUserId();
            await _userTaskService.CreateAsync(userTask);
          }
        }

        if (deserializedSelectedTagIds != null && deserializedSelectedTagIds.Any())
        {
          foreach (int tagId in deserializedSelectedTagIds)
          {
            ToDoTag taskTag = new ToDoTag();
            taskTag.TaskId = taskItem.ToDoID;
            taskTag.TagId = tagId;
            taskTag.OrganizationId = GetOrganizationId();
            await _toDoTagService.CreateAsync(taskTag);
          }
        }

        scope.Complete();
      }

      return RedirectToAction("ToDos");
    }

    [HttpGet]
    [Route("details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
      ToDo taskItem = await _toDoService.GetAsync(id, GetOrganizationId());

      MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

      ToDoViewModel taskViewModel = await ConvertToTaskViewModel(taskItem, _toDoService, pipeline);

      return View(taskViewModel);
    }

    private async Task<ToDoViewModel> ConvertToTaskViewModel(ToDo task, ToDoService taskService, MarkdownPipeline pipeline)
    {
      var taskViewModel = new ToDoViewModel()
      {
        ToDoID = task.ToDoID,
        Title = task.Title,
        HtmlContent = task.Content != null ? Markdown.ToHtml(task.Content, pipeline) : string.Empty,
        MarkdownContent = task.Content,
        ParentToDoId = task.ParentToDoId,
        Created = task.Created,
        Children = new List<ToDoViewModel>()
      };

      var children = await taskService.GetChildrenAsync(task.ToDoID, GetOrganizationId());
      foreach (var child in children)
      {
        taskViewModel.Children.Add(await ConvertToTaskViewModel(child, taskService, pipeline));
      }

      return taskViewModel;
    }
  }
}