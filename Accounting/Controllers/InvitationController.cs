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
        [AllowAnonymous]
        [HttpGet]
        [Route("invitation/{guid}")]
        public async Task<IActionResult> Invitation(Guid guid)
        {
            InvitationService invitationService = new InvitationService();
            Invitation invitation = await invitationService.GetAsync(guid);

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
            InvitationService invitationService = new InvitationService();
            Invitation invitation = await invitationService.GetAsync(model.Guid);

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

            UserService userService = new UserService();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await userService.UpdatePasswordAsync(invitation.UserId, PasswordStorage.CreateHash(model.Password));
                await invitationService.DeleteAsync(model.Guid);
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