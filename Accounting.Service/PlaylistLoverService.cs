using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Accounting.Common;
using SpotifyAPI.Web;
using Accounting.Business;

namespace Accounting.Service
{
  public class PlaylistLoverService : BaseService
  {
    private static readonly HttpClient httpClient = new HttpClient();

    public PlaylistLoverService() : base()
    {

    }

    public PlaylistLoverService(string databaseName, string databasePassword) : base(databaseName, databasePassword)
    {
    }

    private string? ExtractPlaylistId(string playlistInput)
    {
      if (playlistInput.StartsWith("spotify:playlist:"))
      {
        var parts = playlistInput.Split(':');
        if (parts.Length == 3)
          return parts[2];
      }
      else if (playlistInput.Contains("open.spotify.com/playlist/"))
      {
        try
        {
          var uri = new Uri(playlistInput);
          var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
          if (segments.Length >= 2 && segments[0] == "playlist")
            return segments[1];
        }
        catch (UriFormatException)
        {
          // Handle invalid URL
        }
      }
      // Optionally: throw or return null if not valid
      return null;
    }

    public async Task<List<Track>> ExtractTracksFromSpotifyPlaylist(string email, string spotifyOnRepeatSharedPlaylistUri)
    {
      var playlistId = ExtractPlaylistId(spotifyOnRepeatSharedPlaylistUri);

      async Task<FullPlaylist> GetPlaylistWithToken()
      {
        var config = SpotifyClientConfig.CreateDefault().WithToken(ConfigurationSingleton.Instance.SpotifyBearerToken);
        var spotify = new SpotifyClient(config);
        return await spotify.Playlists.Get(playlistId);
      }

      try
      {
        if (string.IsNullOrEmpty(ConfigurationSingleton.Instance.SpotifyBearerToken))
        {
          ConfigurationSingleton.Instance.SpotifyBearerToken
              = await GetSpotifyBearerToken(
                  ConfigurationSingleton.Instance.SpotifyClientID,
                  ConfigurationSingleton.Instance.SpotifyClientSecret);
        }

        FullPlaylist playlist;
        try
        {
          playlist = await GetPlaylistWithToken();
        }
        catch (APIUnauthorizedException)
        {
          // Token expired. Refresh and retry once.
          ConfigurationSingleton.Instance.SpotifyBearerToken
              = await GetSpotifyBearerToken(
                  ConfigurationSingleton.Instance.SpotifyClientID,
                  ConfigurationSingleton.Instance.SpotifyClientSecret);

          playlist = await GetPlaylistWithToken(); // retry once
        }

        var tracks = new List<Track>();
        foreach (var item in playlist.Tracks.Items)
        {
          if (item.Track is FullTrack track)
          {
            tracks.Add(new Track
            {
              SpotifyTrackId = track.Id,
              Title = track.Name,
              Artist = track.Artists != null && track.Artists.Count > 0
                    ? string.Join(", ", track.Artists.Select(a => a.Name))
                    : "",
              Album = track.Album?.Name ?? "",
              Created = DateTime.UtcNow
            });
          }
        }
        return tracks;
      }
      catch (APIUnauthorizedException)
      {
        // If we get here, token refresh didn't help. Inform user appropriately.
        throw new Exception("There was a problem accessing Spotify. Please try again."); // or handle as needed
      }
      catch (APIException ex)
      {
        Console.WriteLine($"Spotify API error: {ex.Message}");
        throw;
      }
    }

    public async Task<string> GetSpotifyBearerToken(string spotifyClientID, string spotifyClientSecret)
    {
      // Combine client ID and client secret and encode them to Base64
      var clientCredentials = $"{spotifyClientID}:{spotifyClientSecret}";
      var clientCredentialsBase64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(clientCredentials));

      // Create request message
      var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
      request.Headers.Authorization = new AuthenticationHeaderValue("Basic", clientCredentialsBase64);

      // Add the grant_type to the body
      var content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
      request.Content = content;

      // Send the request and parse the response
      var response = await httpClient.SendAsync(request);

      if (!response.IsSuccessStatusCode)
      {
        // Handle error response
        throw new Exception($"Error retrieving Spotify token: {response.StatusCode}");
      }

      // Deserialize the response JSON to get the access token
      var responseContent = await response.Content.ReadAsStringAsync();
      var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseContent);
      string accessToken = jsonResponse.access_token;

      return accessToken;
    }

    public async Task<PlaylistLover> GetOrCreateAsync(string email, bool gender)
    {
      throw new NotImplementedException();
    }
  }
}