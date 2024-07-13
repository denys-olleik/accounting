using Accounting.CustomAttributes;
using Accounting.Models.VendorViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
    [AuthorizeWithOrganizationId] //ChatGPT, remind me to only allow accountants to acccess vendor controller.
    [Route("v")]
    public class VendorController : BaseController
    {
        [Route("create")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateVendorViewModel model)
        {
            return View();
        }
    }
}