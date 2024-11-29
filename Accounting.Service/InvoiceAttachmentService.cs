using Accounting.Business;
using Accounting.Common;
using Accounting.Database;

namespace Accounting.Service
{
  public class InvoiceAttachmentService
  {
    private readonly string _databaseName;

    public InvoiceAttachmentService(string databaseName = DatabaseThing.DatabaseConstants.Database)
    {
      _databaseName = databaseName;
    }

    public async Task<List<InvoiceAttachment>> GetAllAsync(int[] ids, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceAttachmentManager().GetAllAsync(ids, organizationId);
    }

    public async Task<int> MoveAndUpdateInvoiceAttachmentPathAsync(InvoiceAttachment invoiceAttachment, string path, int organizationId)
    {
      string newPath = Path.Combine(path, invoiceAttachment.StoredFileName);
      System.IO.File.Move(invoiceAttachment.FilePath, newPath);

      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceAttachmentManager().UpdateFilePathAsync(invoiceAttachment.InvoiceAttachmentID, newPath, organizationId);
    }

    public async Task<int> UpdateInvoiceIdAsync(int invoiceAttachmentId, int invoiceId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceAttachmentManager().UpdateInvoiceIdAsync(invoiceAttachmentId, invoiceId, organizationId);
    }

    public async Task<bool> UpdatePrintOrderAsync(int id, int newPrintOrder, int userId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      int attachment = await factoryManager.GetInvoiceAttachmentManager().UpdatePrintOrderAsync(id, newPrintOrder, userId, organizationId);
      return attachment > 0;
    }

    public async Task<InvoiceAttachment> UploadInvoiceAttachmentAsync(Common.File fileUpload, int userId, int organizationId)
    {
      string nameOnDisk = RandomHelper.GenerateSecureAlphanumericString(15) + Path.GetExtension(fileUpload.FileName);

      string temporaryDirectory = ConfigurationSingleton.Instance.TempPath;

      if (!Directory.Exists(temporaryDirectory))
      {
        Directory.CreateDirectory(temporaryDirectory);
      }

      string fullPath = Path.Combine(temporaryDirectory, nameOnDisk);
      using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
      {
        await fileUpload.Stream.CopyToAsync(fileStream);
      }

      var attachment = new InvoiceAttachment
      {
        FileName = fileUpload.FileName,
        StoredFileName = nameOnDisk,
        FilePath = fullPath,
        CreatedById = userId,
        OrganizationId = organizationId
      };

      var factoryManager = new FactoryManager(_databaseName);
      attachment = await factoryManager.GetInvoiceAttachmentManager().CreateAsync(attachment);

      return attachment;
    }

    public async Task<List<InvoiceAttachment>> GetAllAsync(int invoiceId, int organizationId)
    {
      var factoryManager = new FactoryManager(_databaseName);
      return await factoryManager.GetInvoiceAttachmentManager().GetAllAsync(invoiceId, organizationId);
    }
  }
}