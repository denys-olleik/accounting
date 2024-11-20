using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.OrganizationViewModels;
using Accounting.Service;
using Accounting.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("o")]
  public class OrganizationController : BaseController
  {
    private readonly OrganizationService _organizationService;
    private readonly InvoiceService _invoiceService;

    public OrganizationController(OrganizationService organizationService, InvoiceService invoiceService)
    {
      _organizationService = organizationService;
      _invoiceService = invoiceService;
    }

    [Route("update")]
    [HttpGet]
    public async Task<IActionResult> Update()
    {
      Organization organization = await _organizationService.GetAsync(GetOrganizationId(), GetDatabaseName());

      if (organization == null)
      {
        return NotFound();
      }

      UpdateOrganizationViewModel model = new UpdateOrganizationViewModel
      {
        Name = organization.Name!,
        Address = organization.Address!,
        AccountsReceivableEmail = organization.AccountsReceivableEmail!,
        AccountsPayableEmail = organization.AccountsPayableEmail!,
        AccountsReceivablePhone = organization.AccountsReceivablePhone!,
        AccountsPayablePhone = organization.AccountsPayablePhone!,
        Website = organization.Website!
      };

      return View(model);
    }

    [Route("update")]
    [HttpPost]
    public async Task<IActionResult> Update(UpdateOrganizationViewModel model)
    {
      UpdateOrganizationViewModelValidator validator = new UpdateOrganizationViewModelValidator();
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
      {
        var organizationId = GetOrganizationId();

        await _organizationService.UpdateNameAsync(organizationId, model.Name!);
        await _organizationService.UpdateAddressAsync(organizationId, model.Address!);
        await _organizationService.UpdateAccountsReceivableEmailAsync(organizationId, model.AccountsReceivableEmail!);
        await _organizationService.UpdateAccountsPayableEmailAsync(organizationId, model.AccountsPayableEmail!);
        await _organizationService.UpdateAccountsReceivablePhoneAsync(organizationId, model.AccountsReceivablePhone!);
        await _organizationService.UpdateAccountsPayablePhoneAsync(organizationId, model.AccountsPayablePhone!);
        await _organizationService.UpdateWebsiteAsync(organizationId, model.Website!);

        scope.Complete();
      }

      return RedirectToAction("Index", "Home");
    }
  }
}