using Accounting.Business;
using Accounting.Models.HomeViewModels;
using Accounting.Service;
using Ganss.Xss;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  public class HomeController : BaseController
  {
    private readonly BlogService _blogService;

    public HomeController(RequestContext requestContext, BlogService blogService)
    {
      _blogService = new BlogService(
        requestContext.DatabaseName,
        requestContext.DatabasePassword);
    }

    public async Task<IActionResult> Index()
    {
      Blog latestPublicPost = await _blogService.GetFirstPublicAsync();

      var markdownPipeline = new Markdig.MarkdownPipelineBuilder().Build();

      LatestPostViewModel indexHomeViewModel = new LatestPostViewModel
      {
        Title = latestPublicPost?.Title,
        BlogHtmlSanitizedContent = latestPublicPost?.Content != null
          ? new HtmlSanitizer().Sanitize(
              Markdig.Markdown.ToHtml(latestPublicPost.Content, markdownPipeline))
          : null,
      };

      return View(indexHomeViewModel);
    }

    [HttpGet("unauthorized")]
    public IActionResult Unauthorized()
    {
      return View("Unauthorized");
    }

    [Route("careers")]
    public IActionResult Careers()
    {
      return View();
    }
  }
}