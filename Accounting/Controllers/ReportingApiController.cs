using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Service;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [ApiController]
  [Route("api/r")]
  public class ReportingApiController : BaseController
  {
    private readonly InvoiceService _invoiceService;
    private readonly AccountService _accountService;

    public ReportingApiController(InvoiceService invoiceService, AccountService accountService, RequestContext requestContent)
    {
      _invoiceService = invoiceService;
      _accountService = accountService;
    }

    [HttpGet("get-unpaid-and-paid-balance")]
    public async Task<IActionResult> GetUnpaidAndPaidBalance()
    {
      (decimal unpaid, decimal paid) = await _invoiceService.GetUnpaidAndPaidBalance(GetOrganizationId());

      return Ok(new { unpaid, paid });
    }

    [HttpGet("get-account-balance-report")]
    public async Task<IActionResult> GetAccountBalanceReport()
    {
      var accountBalances = await _accountService.GetAccountBalanceReport(GetOrganizationId());

      return Ok(accountBalances);
    }
  }
}