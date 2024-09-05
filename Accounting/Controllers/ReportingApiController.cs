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
    private readonly AccountService _chartOfAccountService;

    public ReportingApiController(InvoiceService invoiceService, AccountService chartOfAccountService)
    {
      _invoiceService = invoiceService;
      _chartOfAccountService = chartOfAccountService;
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
      var accountBalances = await _chartOfAccountService.GetAccountBalanceReport(GetOrganizationId());

      return Ok(accountBalances);
    }
  }
}