using DSharpPlus.Lavalink;
using Newtonsoft.Json;
using Skynet.db;
using Skynet.Domain;
using Skynet.Services.Interface;
using System.Xml.Linq;

namespace Skynet.Services
{
    public class SearchEngine : ISearchEngine
    {
        public async Task<LavalinkTrack?> GetTracksFromGuildPlaylistAsync(LavalinkNodeConnection node)
        {
            var playlist = ReaderJson.DeserializeFile<List<LavalinkTrack>>("MusicPlaylist");
            if (playlist.Count <= 0) { return default; }
            var searchQuery = await node.Rest.GetTracksAsync(playlist[0].Uri);
            if (searchQuery.LoadResultType == LavalinkLoadResultType.NoMatches || searchQuery.LoadResultType == LavalinkLoadResultType.LoadFailed)
            {
                throw new Exception("Failed to find music corresponding to your query");
            }
            var musicTrack = searchQuery.Tracks.First();
            playlist.Remove(playlist[0]);
            ReaderJson.SerialiseAndSave(playlist, "MusicPlaylist");
            return musicTrack;
        }

        public async Task<LavalinkTrack?> GetRandomTrackAsync(LavalinkNodeConnection node)
        {
            var random = new Random(); 
            var searchTerms = ReaderJson.DeserializeFile<List<MusicSearchTerm>>("MusicSearchTerms");
            var searchTermId = random.Next(0, searchTerms.Count-1);

            var searchQuery = await node.Rest.GetTracksAsync(searchTerms[searchTermId].Term);
            if (searchQuery.LoadResultType == LavalinkLoadResultType.NoMatches || searchQuery.LoadResultType == LavalinkLoadResultType.LoadFailed)
            {
                throw new Exception("Failed to find music corresponding to your query");
            }
            return searchQuery.Tracks.FirstOrDefault();
        } 
    }
}
