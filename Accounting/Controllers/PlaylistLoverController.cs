using Accounting.Models.PlaylistLoverViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
  [Route("playlist-lovers")]
  public class PlaylistLoverController : BaseController
  {
    [HttpGet("process-lover")]
    public IActionResult ProcessLover()
    {
      return View();
    }

    [HttpPost("process-lover")]
    public async Task<IActionResult> ProcessLover(ProcessLoverViewModel lover)
    {
      ProcessLoverViewModel.ProcessLoverViewModelValidator validator = new();
      var validationResult = await validator.ValidateAsync(lover);

      if (!validationResult.IsValid)
      {
        lover.ValidationResult = validationResult;
        return View(lover);
      }

      throw new NotImplementedException();
    }
  }
}