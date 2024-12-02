using Accounting.Business;
using Accounting.Common;
using Accounting.Models.InvitationViewModels;
using Accounting.Service;
using Accounting.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace Accounting.Controllers
{
  [Authorize]
  [Route("i")]
  public class InvitationController : BaseController
  {
    private readonly InvitationService _invitationService;
    private readonly UserService _userService;

    public InvitationController(
      InvitationService invitationService, 
      UserService userService)
    {
      _invitationService = invitationService;
      _userService = userService;
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("invitation/{guid}")]
    public async Task<IActionResult> Invitation(Guid guid)
    {
      Invitation invitation = await _invitationService.GetAsync(guid);

      bool invitationDoesNotExist = invitation == null;
      bool invitationHasExpired = invitation?.Expiration != null && invitation.Expiration < DateTime.UtcNow;

      if (invitationDoesNotExist || invitationHasExpired)
      {
        return RedirectToAction("Invalid");
      }

      InvitationViewModel invitationViewModel = new InvitationViewModel();
      invitationViewModel.Email = invitation.Email;
      invitationViewModel.Guid = invitation.Guid;

      return View(invitationViewModel);
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("invitation/{guid}")]
    public async Task<IActionResult> Invitation(InvitationViewModel model)
    {
      Invitation invitation = await _invitationService.GetAsync(model.Guid);

      if (invitation.Expiration != null && invitation.Expiration < DateTime.UtcNow)
      {
        return RedirectToAction("Invalid");
      }

      InvitationViewModelValidator invitationViewModelValidator = new InvitationViewModelValidator();
      ValidationResult validationResult = await invitationViewModelValidator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        await _userService.UpdatePasswordAllTenantsAsync(invitation.Email, PasswordStorage.CreateHash(model.Password));
        await _invitationService.DeleteAsync(model.Guid);
        scope.Complete();
      }

      return RedirectToAction("Completed");
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("invalid")]
    public async Task<IActionResult> Invalid()
    {
      return View();
    }

    [AllowAnonymous, HttpGet, Route("completed")]
    public async Task<IActionResult> Completed()
    {
      return View();
    }
  }
}