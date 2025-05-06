using Accounting.Business;
using Accounting.Common;
using Accounting.Database;

namespace Accounting.Service
{
  public class InvoiceAttachmentService : BaseService
  {
    public InvoiceAttachmentService() : base()
    {
      
    }

    public InvoiceAttachmentService(
      string databaseName, 
      string databasePassword) : base(databaseName, databasePassword)
    {

    }

    public async Task<List<InvoiceAttachment>> GetAllAsync(int[] ids, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetInvoiceAttachmentManager().GetAllAsync(ids, organizationId);
    }

    public async Task<int> MoveAndUpdateInvoiceAttachmentPathAsync(InvoiceAttachment invoiceAttachment, string destinationPath, int organizationId, string databaseName)
    {
      string dbSpecificPath = Path.Combine(destinationPath, databaseName);
      if (!Directory.Exists(dbSpecificPath))
      {
        Directory.CreateDirectory(dbSpecificPath);
      }

      string fileName = Path.GetFileName(invoiceAttachment.FilePath);
      string newPath = Path.Combine(dbSpecificPath, fileName);
      System.IO.File.Move(invoiceAttachment.FilePath, newPath);

      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetInvoiceAttachmentManager().UpdateFilePathAsync(invoiceAttachment.InvoiceAttachmentID, newPath, organizationId);
    }

    public async Task<int> UpdateInvoiceIdAsync(int invoiceAttachmentId, int invoiceId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetInvoiceAttachmentManager().UpdateInvoiceIdAsync(invoiceAttachmentId, invoiceId, organizationId);
    }

    public async Task<bool> UpdatePrintOrderAsync(int id, int newPrintOrder, int userId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      int attachment = await factoryManager.GetInvoiceAttachmentManager().UpdatePrintOrderAsync(id, newPrintOrder, userId, organizationId);
      return attachment > 0;
    }

    public async Task<InvoiceAttachment> UploadInvoiceAttachmentAsync(Common.File fileUpload, int userId, int organizationId, string databaseName)
    {
      string nameOnDisk = RandomHelper.GenerateSecureAlphanumericString(15) + Path.GetExtension(fileUpload.FileName);

      string temporaryDirectory = ConfigurationSingleton.Instance.TempPath;

      string databaseDirectory = Path.Combine(temporaryDirectory, databaseName);
      if (!Directory.Exists(databaseDirectory))
      {
        Directory.CreateDirectory(databaseDirectory);
      }

      string fullPath = Path.Combine(databaseDirectory, nameOnDisk);
      using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
      {
        await fileUpload.Stream.CopyToAsync(fileStream);
      }

      var attachment = new InvoiceAttachment
      {
        OriginalFileName = fileUpload.FileName,
        FilePath = fullPath,
        CreatedById = userId,
        OrganizationId = organizationId
      };

      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      attachment = await factoryManager.GetInvoiceAttachmentManager().CreateAsync(attachment);

      return attachment;
    }

    public async Task<List<InvoiceAttachment>> GetAllAsync(int invoiceId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      return await factoryManager.GetInvoiceAttachmentManager().GetAllAsync(invoiceId, organizationId);
    }

    public async Task DeleteAttachmentsAsync(List<int> invoiceAttachmentIds, int invoiceID, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName, _databasePassword);
      var invoiceAttachmentManager = factoryManager.GetInvoiceAttachmentManager();

      var attachments = await invoiceAttachmentManager.GetAllAsync(invoiceAttachmentIds.ToArray(), organizationId);

      foreach (var attachment in attachments)
      {
        if (attachment != null)
        {
          if (System.IO.File.Exists(attachment.FilePath))
            System.IO.File.Delete(attachment.FilePath);

          await invoiceAttachmentManager.DeleteAsync(attachment.InvoiceAttachmentID, invoiceID, organizationId);
        }
      }
    }
  }
}