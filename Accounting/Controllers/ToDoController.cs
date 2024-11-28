﻿using Accounting.Business;
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
      TagService tagService = new TagService(GetDatabaseName());
      List<Tag> tags = await tagService.GetAllAsync();

      UserService userService = new UserService(GetDatabaseName());

      CreateToDoViewModel createToDoViewModel = new CreateToDoViewModel();
      createToDoViewModel.Users = (await userService.GetAllAsync(GetOrganizationId())).Select(user => new UserViewModel
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

      ToDoService toDoService = new ToDoService(GetDatabaseName());

      createToDoViewModel.ParentToDoId = parentToDoId;

      ToDo? toDo = parentToDoId.HasValue ? await toDoService.GetAsync(parentToDoId.Value, GetOrganizationId()) : null;
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

      UserService userService = new UserService(GetDatabaseName());

      ToDoService toDoService = new ToDoService(GetDatabaseName());
      ToDo? parentToDoItem = model.ParentToDoId.HasValue ? await toDoService.GetAsync(model.ParentToDoId.Value, GetOrganizationId()) : null;
      model.ParentToDo = parentToDoItem != null ? new ToDoViewModel
      {
        ToDoID = parentToDoItem.ToDoID,
        Title = parentToDoItem.Title,
        HtmlContent = parentToDoItem.Content
      } : null;

      if (!validationResult.IsValid)
      {
        TagService tagService = new TagService(GetDatabaseName());
        List<Tag> tags = await tagService.GetAllAsync();

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

        model.Users = (await userService.GetAllAsync(GetOrganizationId())).Select(user => new UserViewModel
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

      ToDoTagService taskTagService = new ToDoTagService(GetDatabaseName());
      UserTaskService userTaskService = new UserTaskService(GetDatabaseName());
      using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        ToDo taskItem = await toDoService.CreateAsync(new ToDo()
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
            await userTaskService.CreateAsync(userTask);
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
            await taskTagService.CreateAsync(taskTag);
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
      ToDoService taskService = new ToDoService(GetDatabaseName());
      ToDo taskItem = await taskService.GetAsync(id, GetOrganizationId());

      MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

      ToDoViewModel taskViewModel = await ConvertToTaskViewModel(taskItem, taskService, pipeline);

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