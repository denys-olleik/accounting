﻿using Accounting.Models.Account;
using FluentValidation.Results;

namespace Accounting.Models.AccountsReceivableViewModels
{
  public class ReceivePaymentForInvoiceIdsViewModel
  {
    public string? ReferenceNumber { get; set; }
    public List<ReceivePaymentForInvoiceViewModel> Invoices { get; set; }
      = new List<ReceivePaymentForInvoiceViewModel>();
    public List<AccountViewModel>? DebitAccounts { get; set; }
    public decimal PaymentTotal { get; set; }
    public string? SelectedDebitAccountId { get; set; }
    public bool Remember { get; set; }
    public DebitAccountModel? SelectedDebitAccount { get; set; }

    public ValidationResult? ValidationResult { get; internal set; }

    public class DebitAccountModel
    {
      public int AccountID { get; set; }
      public string? Name { get; set; }
    }
  }
}