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
    private readonly TrackService _trackService;
    private readonly PlaylistSubmissionService _submissionService;

    public PlaylistLoverController()
    {
      _playlistLoverService = new();
      _trackService = new();
      _submissionService = new();
    }

    [HttpGet("process-lover")]
    public IActionResult ProcessLover()
    {
      return View();
    }

    [HttpPost("process-lover")]
    public async Task<IActionResult> ProcessLover(ProcessLoverViewModel lover)
    {
      var validator = new ProcessLoverViewModel.ProcessLoverViewModelValidator();
      var validationResult = await validator.ValidateAsync(lover);

      if (!validationResult.IsValid)
      {
        lover.ValidationResult = validationResult;
        return View(lover);
      }

      // Ensure the user exists or create them
      var playlistLover = await _playlistLoverService.GetOrCreateAsync(lover.Email, lover.Gender);

      // Extract tracks from Spotify playlist
      var tracks = await _playlistLoverService.ExtractTracksFromSpotifyPlaylist(lover.Email, lover.Address);

      // Upsert tracks (add only if not existing)
      var trackEntities = await _trackService.UpsertRangeAsync(tracks);

      // Create a new submission
      var submission = await _submissionService.CreateSubmissionAsync(playlistLover.PlaylistLoverID);

      // Link tracks to submission
      await _submissionService.AddTracksToSubmissionAsync(submission.PlaylistSubmissionID, trackEntities.Select(t => t.TrackID).ToList());

      // Optionally: Update PlaylistTrack to reflect current playlist if desired

      throw new NotImplementedException();
    }
  }
}