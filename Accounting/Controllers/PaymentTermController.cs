using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.PaymentTermViewModels;
using Accounting.Service;
using Accounting.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("pt")]
  public class PaymentTermController : BaseController
  {
    private readonly PaymentTermsService _paymentTermsService;

    public PaymentTermController(PaymentTermsService paymentTermsService, RequestContext requestContext)
    {
      _paymentTermsService = new PaymentTermsService(requestContext.DatabasePassword, requestContext.DatabaseName);
    }

    [Route("payment-terms")]
    [HttpGet]
    public async Task<IActionResult> PaymentTerms()
    {
      PaymentTermsViewModel paymentTermsViewModel = new PaymentTermsViewModel();

      List<PaymentTerm> paymentTerms = await _paymentTermsService.GetAllAsync();

      paymentTermsViewModel.PaymentTerms = paymentTerms.Select(paymentTerm => new PaymentTermViewModel
      {
        ID = paymentTerm.PaymentTermID,
        Description = paymentTerm.Description,
        DaysUntilDue = paymentTerm.DaysUntilDue
      }).ToList();

      return View(paymentTermsViewModel);
    }

    [Route("create")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      return View();
    }

    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> Create(CreatePaymentTermsViewModel model)
    {
      CreatePaymentTermsViewModelValidator validator = new CreatePaymentTermsViewModelValidator();
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      await _paymentTermsService.CreatePaymentTermAsync(new PaymentTerm()
      {
        Description = model.Description,
        DaysUntilDue = model.DaysUntilDue,
        CreatedById = GetUserId(),
        OrganizationId = GetOrganizationId()
      });

      return RedirectToAction("PaymentTerms");
    }
  }
}