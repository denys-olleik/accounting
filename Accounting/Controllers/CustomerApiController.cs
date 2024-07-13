using Accounting.CustomAttributes;
using Accounting.Models.BusinessEntityViewModels;
using Accounting.Service;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
    [AuthorizeWithOrganizationId]
    [ApiController]
    [Route("api/c")]
    public class CustomerApiController : BaseController
    {
        [HttpGet("get-customers")]
        public async Task<IActionResult> GetCustomers(int page = 1, int pageSize = 2)
        {
            BusinessEntityService customerService = new BusinessEntityService();
            var (businessEntities, nextPageNumber) = await customerService.GetAllAsync(page, pageSize, GetOrganizationId());

            var getCustomersViewModel = new GetBusinessEntitiesViewModel
            {
                BusinessEntities = businessEntities.Select(c => new BusinessEntityViewModel
                {
                    ID = c.BusinessEntityID,
                    RowNumber = c.RowNumber,
                    CustomerType = c.CustomerType,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    CompanyName = c.CompanyName,
                }).ToList(),
                CurrentPage = page,
                NextPage = nextPageNumber,
            };

            return Ok(getCustomersViewModel);
        }
    }
}