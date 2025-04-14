using Accounting.Common;
using Accounting.Models.PlaylistLoverViewModels;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Accounting.Service;
using Accounting.Business;

namespace Accounting.Controllers
{
  [Route("playlist-lovers")]
  public class PlaylistLoverController : BaseController
  {
    private readonly PlaylistLoverService _playlistLoverService;
    private readonly TrackService trackService;

    public PlaylistLoverController()
    {
      _playlistLoverService = new();
    }

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

      List<Track> tracks = await _playlistLoverService.ExtractTracksFromSpotifyPlaylist(lover.Email, lover.Address);



      throw new NotImplementedException();
    }
  }
}