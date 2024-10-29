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
    IJournalManager GetJournalManager();
    IAccountManager GetAccountManager();
    IPaymentManager GetPaymentManager();
    IInvoiceInvoiceLinePaymentManager GetInvoiceInvoiceLinePaymentManager();
    IJournalInvoiceInvoiceLinePaymentManager GetJournalInvoiceInvoiceLinePaymentManager();
    IUserOrganizationManager GetUserOrganizationManager();
    IOrganizationManager GetOrganizationManager();
    IInvoiceAttachmentManager GetInvoiceAttachmentManager();
    IReconciliationTransactionManager GetReconciliationTransactionManager();
    IReconciliationManager GetReconciliationManager();
    IReconiliationAttachmentManager GetReconiliationAttachmentManager();
    IReconciliationExpenseManager GetExpenseManager();
    IJournalReconciliationTransactionManager GetJournalReconciliationExpenseManager();
    IReconciliationExpenseCategoryManager GetReconciliationExpenseCategoryManager();
    IDatabaseManager GetDatabaseManager();
    IJournalInvoiceInvoiceLineManager GetJournalInvoiceInvoiceLineManager();
    ILocationManager GetLocationManager();
    IInventoryManager GetInventoryManager();
    IRequestLogManager GetRequestLogManager();
    IInventoryAdjustmentManager GetInventoryAdjustmentManager();
    IZIPCodeManager GetZIPCodeManager();
    ITenantManager GetTenantManager();
    ISecretManager GetSecretManager();
    IApplicationSettingManager GetApplicationSettingManager();
    ILoginWithoutPasswordManager GetLoginWithoutPasswordManager();
  }
}