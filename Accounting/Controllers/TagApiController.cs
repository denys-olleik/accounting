using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.TagViewModels;
using Accounting.Service;
using Accounting.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [ApiController]
  [Route("api/tag")]
  public class TagApiController : BaseController
  {
    private readonly TagService _tagService;
    private readonly string _databaseName;

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateTagApiViewModel model)
    {
      CreateTagApiViewModelValidator validator = new CreateTagApiViewModelValidator();
      ValidationResult validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage);
        return BadRequest(new { errors = errorMessages });
      }

      Tag tag = await _tagService.CreateAsync(new Tag
      {
        Name = model.Name,
        CreatedById = GetUserId(),
        OrganizationId = GetOrganizationId()
      });

      return Ok(tag);
    }
  }
}