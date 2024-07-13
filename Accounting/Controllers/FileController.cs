using CsvHelper;
using Accounting.Common;
using Accounting.CustomAttributes;
using Accounting.Service;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Accounting.Controllers
{
    [AuthorizeWithOrganizationId]
    [Route("file")]
    public class FileController : BaseController
    {
        
    }
}