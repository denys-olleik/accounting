using Accounting.Business;
using Accounting.Common;
using Accounting.CustomAttributes;
using Accounting.Models;
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
      if (!string.IsNullOrWhiteSpace(ConfigurationSingleton.Instance.Whitelabel))
      {
        return WhiteLabelIndex();
      }

      Blog latestPublicPost = await _blogService.GetFirstPublicAsync();

      var markdownPipeline = new Markdig.MarkdownPipelineBuilder().Build();

      LatestPostViewModel indexHomeViewModel = new LatestPostViewModel
      {
        Title = latestPublicPost?.Title,
        Created = latestPublicPost?.Created,
        BlogHtmlSanitizedContent = latestPublicPost?.Content != null
          ? new HtmlSanitizer().Sanitize(
              Markdig.Markdown.ToHtml(latestPublicPost.Content, markdownPipeline))
          : null,
      };

      return View(indexHomeViewModel);
    }

    public IActionResult WhiteLabelIndex()
    {
      return View("WhiteLabelIndex");
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

  [AuthorizeWithOrganizationId]
  [Route("api/player")]
  [ApiController]
  public class PlayerApiController : BaseController
  {
    private readonly PlayerService _playerService;
    public PlayerApiController(RequestContext requestContext, PlayerService playerService)
    {
      _playerService = new PlayerService(
        requestContext.DatabaseName,
        requestContext.DatabasePassword);
    }

    [HttpPost("request-position")]
    public async Task<IActionResult> RequestPosition(RequestPositionModel model)
    {
      Guid? guid = null;
      if (Request.Cookies.TryGetValue("PlayerGuid", out string guidString) && Guid.TryParse(guidString, out Guid parsedGuid))
        guid = parsedGuid;
      else
      {
        guid = Guid.NewGuid();
        Response.Cookies.Append("PlayerGuid", guid.ToString(), new CookieOptions { HttpOnly = true });
      }

      Player playerResult = await _playerService.RequestPosition(guid.Value, GetClientIpAddress());
      var boardState = await _playerService.GetFullBoardState(playerResult.Country); 

      return Ok(new
      {
        player = new
        {
          guid = guid,
          position = new { x = playerResult.CurrentX, y = playerResult.CurrentY },
          assignmentStatus = playerResult.AssignmentStatus // e.g., "confirmed", "pending"
        },
        board = boardState // e.g., list or map of all occupied cells and their info
      });
    }
  }
}