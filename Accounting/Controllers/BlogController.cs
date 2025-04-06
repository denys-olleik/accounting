using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.BlogViewModels;
using Accounting.Service;
using Microsoft.AspNetCore.Mvc;
using Ganss.Xss;
using Accounting.Common;

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

    [HttpGet("delete/{blogID}")]
    public async Task<IActionResult> Delete(int blogID)
    {
      Blog blog = await _blogService.GetAsync(blogID);

      if (blog == null)
      {
        return NotFound();
      }

      var deleteBlogViewModel = new DeleteBlogViewModel
      {
        BlogID = blog.BlogID,
        Title = blog.Title,
        Content = blog.Content
      };

      return View(deleteBlogViewModel);
    }

    [HttpPost("delete/{blogID}")]
    public async Task<IActionResult> Delete(DeleteBlogViewModel model)
    {
      await _blogService.DeleteAsync(model.BlogID);
      return RedirectToAction("Blogs");
    }

    [Route("create")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      return View();
    }

    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateBlogViewModel createBlogViewModel)
    {
      var validator = new CreateBlogViewModel.CreateBlogViewModelValidator();
      var validationResult = await validator.ValidateAsync(createBlogViewModel);
      
      if (!validationResult.IsValid)
      {
        createBlogViewModel.ValidationResult = validationResult;
        return View(createBlogViewModel);
      }

      var blog = new Blog
      {
        Title = createBlogViewModel.Title,
        Content = createBlogViewModel.Content,
        CreatedById = GetUserId(),
      };

      if (createBlogViewModel.Public)
      {
        blog.PublicId = RandomHelper.GenerateSecureAlphanumericString(10);
        blog.Slug = blog.PublicId;
      }

      await _blogService.CreateAsync(blog);
      return RedirectToAction("Blogs");
    }

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

      var markdownPipeline = new Markdig.MarkdownPipelineBuilder().Build();

      var sanitizer = new HtmlSanitizer();
      GetBlogsViewModel getBlogsViewModel = new GetBlogsViewModel
      {
        Blogs = blogs.Select(b => new GetBlogsViewModel.BlogViewModel
        {
          BlogID = b.BlogID,
          Title = b.Title,
          Content = sanitizer.Sanitize(Markdig.Markdown.ToHtml(b.Content, markdownPipeline)),
          RowNumber = b.RowNumber
        }).ToList(),
        Page = page,
        NextPage = nextPage,
      };

      return Ok(getBlogsViewModel);
    }
  }
}