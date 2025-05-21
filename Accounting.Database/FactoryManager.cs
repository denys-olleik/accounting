using Accounting.Database.Interfaces;
using static Accounting.Business.Claim;

namespace Accounting.Database
{
  public class FactoryManager
  {
    private IDatabaseFactoryDefinition databaseFactoryDefinition;

    public FactoryManager(string databaseName, string databasePassword)
    {
      if (string.IsNullOrEmpty(databasePassword))
      {
        databasePassword = CustomClaimTypeConstants.Password;
      }

      databaseFactoryDefinition = new DapperPsqlFactory(databaseName, databasePassword);
    }

    public IAddressManager GetAddressManager()
    {
      return databaseFactoryDefinition.GetAddressManager();
    }

    public IAccountManager GetAccountManager()
    {
      return databaseFactoryDefinition.GetAccountManager();
    }

    public IBusinessEntityManager GetBusinessEntityManager()
    {
      return databaseFactoryDefinition.GetBusinessEntityManager();
    }

    public IJournalManager GetJournalManager()
    {
      return databaseFactoryDefinition.GetJournalManager();
    }

    public IInvoiceLineManager GetInvoiceLineManager()
    {
      return databaseFactoryDefinition.GetInvoiceLineManager();
    }

    public IInvoiceManager GetInvoiceManager()
    {
      return databaseFactoryDefinition.GetInvoiceManager();
    }

    public IItemManager GetItemManager()
    {
      return databaseFactoryDefinition.GetItemManager();
    }

    public IJournalInvoiceInvoiceLineManager GetJournalInvoiceInvoiceLineManager()
    {
      return databaseFactoryDefinition.GetJournalInvoiceInvoiceLineManager();
    }

    public IPaymentInstructionManager GetPaymentInstructionManager()
    {
      return databaseFactoryDefinition.GetPaymentInstructionManager();
    }

    public IPaymentTermManager GetPaymentTermManager()
    {
      return databaseFactoryDefinition.GetPaymentTermManager();
    }

    public ITagManager GetTagManager()
    {
      return databaseFactoryDefinition.GetTagManager();
    }

    public IToDoManager GetTaskManager()
    {
      return databaseFactoryDefinition.GetTaskManager();
    }

    public IToDoTagManager GetTaskTagManager()
    {
      return databaseFactoryDefinition.GetToDoTagManager();
    }

    public IUserManager GetUserManager()
    {
      return databaseFactoryDefinition.GetUserManager();
    }

    public IUserToDoManager GetUserTaskManager()
    {
      return databaseFactoryDefinition.GetUserToDoManager();
    }

    public IPaymentManager GetPaymentManager()
    {
      return databaseFactoryDefinition.GetPaymentManager();
    }

    public IInvoiceInvoiceLinePaymentManager GetInvoiceInvoiceLinePaymentManager()
    {
      return databaseFactoryDefinition.GetInvoiceInvoiceLinePaymentManager();
    }

    public IJournalInvoiceInvoiceLinePaymentManager GetJournalInvoiceInvoiceLinePaymentManager()
    {
      return databaseFactoryDefinition.GetJournalInvoiceInvoiceLinePaymentManager();
    }

    public IUserOrganizationManager GetUserOrganizationManager()
    {
      return databaseFactoryDefinition.GetUserOrganizationManager();
    }

    public IOrganizationManager GetOrganizationManager()
    {
      return databaseFactoryDefinition.GetOrganizationManager();
    }

    public IInvoiceAttachmentManager GetInvoiceAttachmentManager()
    {
      return databaseFactoryDefinition.GetInvoiceAttachmentManager();
    }

    public IReconciliationTransactionManager GetReconciliationTransactionManager()
    {
      return databaseFactoryDefinition.GetReconciliationTransactionManager();
    }

    public IReconciliationManager ReconciliationManager()
    {
      return databaseFactoryDefinition.GetReconciliationManager();
    }

    public IReconiliationAttachmentManager ReconiliationAttachmentManager()
    {
      return databaseFactoryDefinition.GetReconiliationAttachmentManager();
    }

    public IJournalReconciliationTransactionManager GetJournalReconciliationTransactionManager()
    {
      return databaseFactoryDefinition.GetJournalReconciliationExpenseManager();
    }

    public IReconciliationExpenseManager GetExpenseManager()
    {
      return databaseFactoryDefinition.GetExpenseManager();
    }

    public IReconciliationExpenseCategoryManager GetReconciliationExpenseCategoryManager()
    {
      return databaseFactoryDefinition.GetReconciliationExpenseCategoryManager();
    }

    public IDatabaseManager GetDatabaseManager()
    {
      return databaseFactoryDefinition.GetDatabaseManager();
    }

    public ILocationManager GetLocationService()
    {
      return databaseFactoryDefinition.GetLocationManager();
    }

    public IInventoryManager GetInventoryManager()
    {
      return databaseFactoryDefinition.GetInventoryManager();
    }

    public IRequestLogManager GetRequestLogManager()
    {
      return databaseFactoryDefinition.GetRequestLogManager();
    }

    public IInventoryAdjustmentManager GetInventoryAdjustmentManager()
    {
      return databaseFactoryDefinition.GetInventoryAdjustmentManager();
    }

    public IZIPCodeManager GetZIPCodeManager()
    {
      return databaseFactoryDefinition.GetZIPCodeManager();
    }

    public ITenantManager GetTenantManager()
    {
      return databaseFactoryDefinition.GetTenantManager();
    }

    public ISecretManager GetSecretManager()
    {
      return databaseFactoryDefinition.GetSecretManager();
    }

    public ILoginWithoutPasswordManager GetLoginWithoutPasswordManager()
    {
      return databaseFactoryDefinition.GetLoginWithoutPasswordManager();
    }

    public IBlogManager GetBlogManager()
    {
      return databaseFactoryDefinition.GetBlogManager();
    }

    public IExceptionManager GetExceptionManager()
    {
      return databaseFactoryDefinition.GetExceptionManager();
    }

    public IClaimManager GetClaimManager()
    {
      return databaseFactoryDefinition.GetClaimManager();
    }

    public IPlayerManager GetPlayerManager()
    {
      return databaseFactoryDefinition.GetPlayerManager();
    }
  }
}