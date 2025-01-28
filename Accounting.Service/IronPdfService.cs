using Accounting.Business;
using HandlebarsDotNet;

namespace Accounting.Service
{
  public class IronPdfService
  {
    private readonly InvoiceService _invoiceService;
    private readonly InvoiceAttachmentService _invoiceAttachmentService;

    public IronPdfService(InvoiceService invoiceService, InvoiceAttachmentService invoiceAttachmentService)
    {
      _invoiceService = invoiceService;
      _invoiceAttachmentService = invoiceAttachmentService;
    }

    public async Task<byte[]> PrintInvoice(int invoiceId, int organizationId, bool withAttachments = false)
    {
      string populatedHtml = await GetPopulatedInvoiceHtml(invoiceId, organizationId);

      var renderer = new ChromePdfRenderer();
      var pdf = renderer.RenderHtmlAsPdf(populatedHtml);

      List<InvoiceAttachment> invoiceAttachments = await _invoiceAttachmentService.GetAllAsync(invoiceId, organizationId);

      if (withAttachments && invoiceAttachments.Any())
      {
        List<PdfDocument> pdfDocuments = new List<PdfDocument> { pdf };

        foreach (InvoiceAttachment invoiceAttachment in invoiceAttachments)
        {
          PdfDocument doc = PdfDocument.FromFile(invoiceAttachment.FilePath);
          pdfDocuments.Add(doc);
        }

        pdf = PdfDocument.Merge(pdfDocuments);
      }

      return pdf.BinaryData;
    }

    public async Task<string> GetPopulatedInvoiceHtml(int invoiceId, int organizationId)
    {
      Invoice invoice = await _invoiceService.GetAsync(invoiceId, organizationId);

      string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "invoice-template.html");
      var template = Handlebars.Compile(await System.IO.File.ReadAllTextAsync(templatePath));

      return template(invoice);
    }
  }
}