using Accounting.Business;
using Accounting.Common;
using Accounting.CustomAttributes;
using Accounting.Service;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
    [AuthorizeWithOrganizationId]
    [ApiController]
    [Route("api/invoice-attachment")]
    public class InvoiceAttachmentApiController : BaseController
    {
        [Route("upload")]
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile formFile)
        {
            Common.File fileUpload = new Common.File
            {
                FileName = formFile.FileName,
                Stream = formFile.OpenReadStream()
            };

            InvoiceAttachmentService attachmentService = new InvoiceAttachmentService();
            InvoiceAttachment attachment = await attachmentService.UploadInvoiceAttachmentAsync(fileUpload, GetUserId(), GetOrganizationId());

            return Ok(new { Id = attachment.InvoiceAttachmentID, FileName = attachment.FileName });
        }

        [Route("update-print-order")]
        [HttpPost]
        public async Task<IActionResult> UpdatePrintOrder([FromBody] UpdatePrintOrderModel model)
        {
            InvoiceAttachmentService attachmentService = new InvoiceAttachmentService();
            bool isSuccess = await attachmentService.UpdatePrintOrderAsync(model.ID, model.NewPrintOrder, GetUserId(), GetOrganizationId());

            if (isSuccess)
            {
                return Ok(new { Status = "Success", Message = "Print order updated successfully." });
            }
            else
            {
                return BadRequest(new { Status = "Failed", Message = "Failed to update print order." });
            }
        }
    }

    public class UpdatePrintOrderModel
    {
        public int ID { get; set; }
        public int NewPrintOrder { get; set; }
    }
}