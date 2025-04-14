using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Accounting.Common;
using SpotifyAPI.Web;

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

    public async Task ProcessLover(string email, string spotifyOnRepeatSharedPlaylistUri)
    {
      var playlistId = ExtractPlaylistId(spotifyOnRepeatSharedPlaylistUri);

      if (string.IsNullOrEmpty(ConfigurationSingleton.Instance.SpotifyBearerToken))
      {
        ConfigurationSingleton.Instance.SpotifyBearerToken
            = await GetSpotifyBearerToken(
                ConfigurationSingleton.Instance.SpotifyClientID,
                ConfigurationSingleton.Instance.SpotifyClientSecret);
      }

      var config = SpotifyClientConfig.CreateDefault().WithToken(ConfigurationSingleton.Instance.SpotifyBearerToken);
      var spotify = new SpotifyClient(config);

      try
      {
        FullPlaylist playlist = await spotify.Playlists.Get(playlistId);

        foreach (var item in playlist.Tracks.Items)
        {
          // Each item is a PlaylistTrack<IPlayableItem>
          if (item.Track is FullTrack track)
          {
            string spotifyTrackId = track.Id;
            string title = track.Name;
            string artist = track.Artists != null && track.Artists.Count > 0
                ? string.Join(", ", track.Artists.Select(a => a.Name))
                : "";
            string album = track.Album?.Name ?? "";

            // Example: Console.WriteLine, or insert into DB here
            Console.WriteLine($"Track ID: {spotifyTrackId}, Title: {title}, Artist: {artist}, Album: {album}");
          }
        }
      }
      catch (APIUnauthorizedException)
      {
        Console.WriteLine("Token expired or invalid.");
        // Optionally, handle token refresh here.
      }
      catch (APIException ex)
      {
        Console.WriteLine($"Spotify API error: {ex.Message}");
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
  }
}