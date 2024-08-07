﻿using Accounting.Business;
using Accounting.Common;
using Accounting.CustomAttributes;
using Accounting.Models.UserViewModels;
using Accounting.Service;
using Accounting.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("u")]
  public class UserController : BaseController
  {
    private readonly UserOrganizationService _userOrganizationService;
    private readonly UserService _userService;

    public UserController(UserOrganizationService userOrganizationService, UserService userService)
    {
      _userOrganizationService = userOrganizationService;
      _userService = userService;
    }

    [HttpGet]
    [Route("users")]
    public async Task<IActionResult> Users()
    {
      List<User> users = await _userService.GetAllAsync(GetOrganizationId());

      UsersViewModel usersViewModel = new UsersViewModel();
      usersViewModel.Users = new List<UserViewModel>();
      foreach (var user in users)
      {
        UserViewModel userViewModel = new UserViewModel();
        userViewModel.UserID = user.UserID;
        userViewModel.Email = user.Email;
        userViewModel.FirstName = user.FirstName;
        userViewModel.LastName = user.LastName;
        userViewModel.Claims = new List<Models.ClaimViewModels.ClaimViewModel>();
        usersViewModel.Users.Add(userViewModel);
      }

      return View(usersViewModel);
    }

    [HttpGet]
    [Route("create")]
    public async Task<IActionResult> Create()
    {
      return View();
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult> Create(CreateUserViewModel model)
    {
      CreateUserViewModelValidator validator = new CreateUserViewModelValidator();
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      InvitationService invitationService = new InvitationService();

      EmailService emailService = new EmailService();
      using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        User user = await _userService.CreateAsync(new User()
        {
          Email = model.Email,
          FirstName = model.FirstName,
          LastName = model.LastName,
          CreatedById = GetUserId()
        });

        await _userOrganizationService.CreateAsync(new UserOrganization()
        {
          OrganizationId = GetOrganizationId(),
          UserId = user.UserID
        });

        if (model.SendInvitationEmail)
        {
          Invitation invitation = await invitationService.CreatAsync(new Invitation()
          {
            Email = user.Email,
            Guid = Guid.NewGuid(),
            FirstName = model.FirstName,
            LastName = model.LastName,
            UserId = user.UserID,
            CreatedById = GetUserId(),
            OrganizationId = GetOrganizationId(),
            Expiration = DateTime.UtcNow + TimeSpan.FromMinutes(ConfigurationSingleton.Instance.InvitationExpirationMinutes)
          });

          await emailService.SendInvitationEmailAsync(invitation, GetBaseUrl());
        }

        scope.Complete();
      }

      return RedirectToAction("Users");
    }

    [HttpGet]
    [Route("details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
      User user = (await _userOrganizationService.GetAsync(id, GetOrganizationId())).User!;

      if (user == null)
      {
        return NotFound();
      }

      UserDetailsViewModel userDetailsViewModel = new UserDetailsViewModel();
      userDetailsViewModel.Email = user.Email;
      userDetailsViewModel.FirstName = user.FirstName;
      userDetailsViewModel.LastName = user.LastName;
      userDetailsViewModel.CreatedById = user.CreatedById;
      userDetailsViewModel.Created = user.Created;

      return View(userDetailsViewModel);
    }
  }
}