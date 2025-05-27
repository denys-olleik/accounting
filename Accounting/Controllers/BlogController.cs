using Accounting.Business;
using Accounting.CustomAttributes;
using Accounting.Models.BlogViewModels;
using Accounting.Service;
using Microsoft.AspNetCore.Mvc;
using Ganss.Xss;
using Accounting.Common;
using Markdig;
using Microsoft.AspNetCore.Authorization;

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

    [HttpGet("update/{blogID}")]
    public async Task<IActionResult> Update(int blogID)
    {
      Blog blog = await _blogService.GetAsync(blogID);

      if (blog == null)
      {
        return NotFound();
      }

      var updateBlogViewModel = new UpdateBlogViewModel
      {
        BlogID = blog.BlogID,
        Title = blog.Title,
        Content = blog.Content,
        Public = !string.IsNullOrEmpty(blog.PublicId)
      };

      return View(updateBlogViewModel);
    }

    [HttpPost("update/{blogID}")]
    public async Task<IActionResult> Update(UpdateBlogViewModel model)
    {
      var validator = new UpdateBlogViewModel.UpdateBlogViewModelValidator();
      var validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      var blog = new Blog
      {
        BlogID = model.BlogID,
        Title = model.Title,
        Content = model.Content
      };

      if (model.Public)
      {
        blog.PublicId = RandomHelper.GenerateSecureAlphanumericString(10, true);
      }

      await _blogService.UpdateAsync(blog);
      return RedirectToAction("Blogs");
    }

    [AllowAnonymous]
    [HttpGet("view/{id}")]
    public async Task<IActionResult> View(string id)
    {
      Blog blog;

      blog = await _blogService.GetByPublicIdAsync(id);

      if (blog == null)
      {
        return NotFound();
      }

      var markdownPipeline = new MarkdownPipelineBuilder().Build();

      var viewBlogViewModel = new ViewBlogViewModel
      {
        BlogID = blog.BlogID,
        PublicId = blog.PublicId,
        Title = blog.Title,
        Content = blog.Content,
        ContentHtml = new HtmlSanitizer().Sanitize(Markdown.ToHtml(blog.Content, markdownPipeline))
      };

      return View(viewBlogViewModel);
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
    public async Task<IActionResult> Create(CreateBlogViewModel model)
    {
      var validator = new CreateBlogViewModel.CreateBlogViewModelValidator();
      var validationResult = await validator.ValidateAsync(model);

      if (!validationResult.IsValid)
      {
        model.ValidationResult = validationResult;
        return View(model);
      }

      var blog = new Blog
      {
        Title = model.Title,
        Content = model.Content,
        CreatedById = GetUserId(),
      };

      if (model.Public)
      {
        blog.PublicId = RandomHelper.GenerateSecureAlphanumericString(10, true);
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

      var markdownPipeline = new Markdig.MarkdownPipelineBuilder()
          .Build();

      var sanitizer = new HtmlSanitizer();
      GetBlogsViewModel getBlogsViewModel = new GetBlogsViewModel
      {
        Blogs = blogs.Select(b => new GetBlogsViewModel.BlogViewModel
        {
          BlogID = b.BlogID,
          PublicId = b.PublicId,
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