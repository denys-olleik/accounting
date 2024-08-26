using Accounting.Database.Interfaces;

namespace Accounting.Database
{
  public interface IDatabaseFactoryDefinition
  {
    IUserManager GetUserManager();
    IToDoManager GetTaskManager();
    IUserToDoManager GetUserToDoManager();
    IInvitationManager GetInvitationManager();
    ITagManager GetTagManager();
    IToDoTagManager GetToDoTagManager();
    IBusinessEntityManager GetBusinessEntityManager();
    IPaymentTermManager GetPaymentTermManager();
    IPaymentInstructionManager GetPaymentInstructionManager();
    IAddressManager GetAddressManager();
    IInvoiceManager GetInvoiceManager();
    IItemManager GetItemManager();
    IInvoiceLineManager GetInvoiceLineManager();
    IGeneralLedgerManager GetGeneralLedgerManager();
    IChartOfAccountManager GetChartOfAccountManager();
    IPaymentManager GetPaymentManager();
    IInvoiceInvoiceLinePaymentManager GetInvoiceInvoiceLinePaymentManager();
    IGeneralLedgerInvoiceInvoiceLinePaymentManager GetGeneralLedgerInvoiceInvoiceLinePaymentManager();
    IUserOrganizationManager GetUserOrganizationManager();
    IOrganizationManager GetOrganizationManager();
    IInvoiceAttachmentManager GetInvoiceAttachmentManager();
    IReconciliationTransactionManager GetReconciliationTransactionManager();
    IReconciliationManager GetReconciliationManager();
    IReconiliationAttachmentManager GetReconiliationAttachmentManager();
    IReconciliationExpenseManager GetExpenseManager();
    IGeneralLedgerReconciliationTransactionManager GetGeneralLedgerReconciliationExpenseManager();
    IReconciliationExpenseCategoryManager GetReconciliationExpenseCategoryManager();
    IDatabaseManager GetDatabaseManager();
    IGeneralLedgerInvoiceInvoiceLineManager GetGeneralLedgerInvoiceInvoiceLineManager();
    ILocationManager GetLocationManager();
    IInventoryManager GetInventoryManager();
    IRequestLogManager GetRequestLogManager();
    IInventoryAdjustmentManager GetInventoryAdjustmentManager();
    IZIPCodeManager GetZIPCodeManager();
    ITenantManager GetTenantManager();
    ISecretManager GetSecretManager();
  }
}