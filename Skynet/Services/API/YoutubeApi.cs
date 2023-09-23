using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Skynet.Domain.Youtube;

namespace Skynet.Services.API
{
    public class YoutubeApiClient : IYoutubeApiClient
    {
        private readonly IHttpClientFactory _httpClient;
        //public YoutubeApiClient(IHttpClientFactory httpClientFactory)
        //{
        //    _httpClient = httpClientFactory;
        //}

        public async Task GetPlaylistAsync()
        {

            //var requestBase = new YouTubeBaseServiceRequest(); 
            //var resource = new YoutubeResource();

            // Create the service.
            var service = new YouTubeService(new BaseClientService.Initializer
            {
                ApplicationName = "Skynet",
                ApiKey = YoutubeApiConfig.YoutubeAppKey,
            });
            var request = "https://youtube.googleapis.com/youtube/v3/playlists?id=PLbpi6ZahtOH7c6nDA9YG3QcyRGbZ4xDFn&key=AIzaSyBcDF1izJ5uJVafzOo5YG7LLsu0_QHGKgA&part=contentDetails";

            // Run the request.
            Console.WriteLine("Executing a list request...");
            var createdRequest = service.Playlists.List("contentDetails").CreateRequest();
            var result = await service.HttpClient.GetAsync(request);
            //tHIS IS A VALID LINK TO RETRIEVE PLAYLIST DETAILS.
            //
            //// Display the results.
            //if (result.Items != null && result.Items.Count >0)
            //{
            //    foreach (var songs in result.Items)
            //    {
            //        Console.WriteLine(songs.ContentDetails);
            //    }
            //}

            // try
            // {
            //     var response = await _httpClient.CreateClient(YoutubeApiConfig.YoutubeApiClientName).GetAsync(
            //$"https://developers.google.com/apis-explorer/#p/youtube/v3/youtube.playlists.list?part=contentDetails&id=PLOU2XLYxmsIIM9h1Ybw2DuRw6o2fkNMeR,PLyYlLs02rgBYRWBzYpoHz7m2SE8mEZ68w");
            //     var responseBody = await response.Content.ReadAsStringAsync();
            //     var deserialised = JsonConvert.DeserializeObject<SteamResponseWrapper>(responseBody);

            //     if (deserialised != null)
            //     {
            //         var player = deserialised.Response.Players[0];
            //         return;
            //     }
            //     else
            //     {
            //         throw new Exception("Steam provided incorrect data");
            //     }
            // }
            // catch (Exception e)
            // {
            //     throw;
            // }
        }
        private async Task RuPlaylistUpdates()
        {
            UserCredential credential;
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    // This OAuth 2.0 access scope allows for full read/write access to the
                    // authenticated user's account.
                    new[] { YouTubeService.Scope.Youtube },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(GetType().ToString())
                );
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = GetType().ToString()
            });

            // Create a new, private playlist in the authorized user's channel.
            var newPlaylist = new Playlist();
            newPlaylist.Snippet = new PlaylistSnippet();
            newPlaylist.Snippet.Title = "Test Playlist";
            newPlaylist.Snippet.Description = "A playlist created with the YouTube API v3";
            newPlaylist.Status = new PlaylistStatus();
            newPlaylist.Status.PrivacyStatus = "public";
            newPlaylist = await youtubeService.Playlists.Insert(newPlaylist, "snippet,status").ExecuteAsync();

            // Add a video to the newly created playlist.
            var newPlaylistItem = new PlaylistItem();
            newPlaylistItem.Snippet = new PlaylistItemSnippet();
            newPlaylistItem.Snippet.PlaylistId = newPlaylist.Id;
            newPlaylistItem.Snippet.ResourceId = new ResourceId();
            newPlaylistItem.Snippet.ResourceId.Kind = "youtube#video";
            newPlaylistItem.Snippet.ResourceId.VideoId = "GNRMeaz6QRI";
            newPlaylistItem = await youtubeService.PlaylistItems.Insert(newPlaylistItem, "snippet").ExecuteAsync();

            Console.WriteLine("Playlist item id {0} was added to playlist id {1}.", newPlaylistItem.Id, newPlaylist.Id);
        }
    }
}
