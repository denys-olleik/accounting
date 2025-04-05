using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.BlogViewModels;
using Accounting.Service;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [AuthorizeWithOrganizationId]
  [Route("blog")]
  public class BlogController : BaseController
  {
    private readonly BlogService _blogService;

    public BlogController(RequestContext requestContext, BlogService blogService)
    {
      _blogService = new BlogService(
        requestContext.DatabaseName,
        requestContext.DatabasePassword);
    }

    [Route("create")]
    [HttpGet]

    [HttpGet]
    [Route("blogs")]
    public IActionResult Blogs(
      int page = 1,
      int pageSize = 2)
    {
      var referer = Request.Headers["Referer"].ToString() ?? string.Empty;

      var vm = new BlogsPaginatedViewModel
      {
        Page = page,
        PageSize = pageSize,
        RememberPageSize = string.IsNullOrEmpty(referer),
      };

      return View(vm);
    }
  }

  [AuthorizeWithOrganizationId]
  [ApiController]
  [Route("api/blog")]
  public class BlogApiController : BaseController
  {
    private readonly BlogService _blogService;

    public BlogApiController(RequestContext requestContext, BlogService blogService)
    {
      _blogService = new BlogService(
        requestContext.DatabaseName,
        requestContext.DatabasePassword);
    }

    [HttpGet("get-blogs")]
    public async Task<IActionResult> GetBlogs(
      int page = 1,
      int pageSize = 2)
    {
      var (blogs, nextPage) = await _blogService.GetAllAsync(page, pageSize);

      GetBlogsViewModel getBlogsViewModel = new GetBlogsViewModel
      {
        Blogs = blogs.Select(b => new  GetBlogsViewModel.BlogViewModel
        {
          BlogID = b.BlogID,
          Title = b.Title,
          Content = b.Content,
        }).ToList(),
        Page = page,
        NextPage = nextPage,
      };

      return Ok(getBlogsViewModel);
    }
  }
}