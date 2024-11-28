using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.PaymentInstructionViewModels;
using Accounting.Service;
using Accounting.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("pi")]
  public class PaymentInstructionController : BaseController
  {
    [Route("payment-instructions")]
    [HttpGet]
    public async Task<IActionResult> PaymentInstructions()
    {
      PaymentInstructionsViewModel paymentInstructionsViewModel = new PaymentInstructionsViewModel();

      PaymentInstructionService paymentInstructionService = new PaymentInstructionService(GetDatabaseName());
      List<PaymentInstruction> paymentInstructions =
          await paymentInstructionService.GetPaymentInstructionsAsync(GetOrganizationId());

      paymentInstructionsViewModel.PaymentInstructions = paymentInstructions.Select(pi => new PaymentInstructionViewModel
      {
        ID = pi.PaymentInstructionID,
        Title = pi.Title,
        Content = pi.Content
      }).ToList();

      return View(paymentInstructionsViewModel);
    }

    [HttpGet]
    [Route("create")]
    public async Task<IActionResult> Create()
    {
      return View();
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create(CreatePaymentInstructionViewModel model)
    {
      PaymentInstructionViewModelValidator validator =
          new PaymentInstructionViewModelValidator();
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      PaymentInstruction paymentInstruction = new PaymentInstruction
      {
        Title = model.Title,
        Content = model.Content,
        CreatedById = GetUserId(),
        OrganizationId = GetOrganizationId()
      };

      PaymentInstructionService paymentInstructionService = new PaymentInstructionService(GetDatabaseName());
      await paymentInstructionService.CreateAsync(paymentInstruction);

      return RedirectToAction("PaymentInstructions");
    }
  }
}