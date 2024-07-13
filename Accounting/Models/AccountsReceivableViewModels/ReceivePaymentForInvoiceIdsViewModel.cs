using Accounting.Models.ChartOfAccount;
using FluentValidation.Results;

namespace Accounting.Models.AccountsReceivableViewModels
{
  public class ReceivePaymentForInvoiceIdsViewModel
  {
    public string? ReferenceNumber { get; set; }
    public List<ReceivePaymentForInvoiceViewModel> Invoices { get; set; }
      = new List<ReceivePaymentForInvoiceViewModel>();
    public List<ChartOfAccountViewModel>? DebitAccounts { get; set; }
    public decimal PaymentTotal { get; set; }
    public string? SelectedDebitAccountId { get; set; }

    public ValidationResult? ValidationResult { get; internal set; }
  }
}