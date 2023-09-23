using DSharpPlus.Lavalink;
using Skynet.Domain.Enum;
using Skynet.Domain.GuildData;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skynet.Domain
{
    public class SearchEngineResult
    {
        public int Id { get; set; }
        public GuildMusicData GuildMusicData { get; set; }
        public int GuildMusicDataId { get; set; }
        [NotMapped]
        public List<LavalinkTrack> Tracks { get; set; }
        public string? SongUri { get; set; }
        public string? SongName { get; set; }
        public string? SongAuthor { get; set; }
        public bool PlaylistReceived { get; set; }
        public string? PlaylistName { get; set; }
        public SearchInput SearchInput { get; set; }
        public SearchType SearchType { get; set; }
        public LavalinkSearchTypeInt LavalinkSearchType { get; set; }
        public DateTime SearchDate { get; set; }
        public bool Successfull { get; set; }
        public SearchEngineResult()
        {
            Tracks = new List<LavalinkTrack>();
        }
        /// <summary>
        /// Updates search result values depending on the found track.
        /// Sets default values for playlist name if none found and updates crucial track info. 
        public void FinaliseSearchResult(bool success)
        {
            Successfull = success;
            SearchDate = DateTime.Now;
            if (!Successfull) { return; }
            SongUri = Tracks.FirstOrDefault().Uri.ToString();
            SongName = Tracks.First().Title;
            SongAuthor = Tracks.First().Author;
            if (!PlaylistReceived) { PlaylistName = ""; }

        }
    }
}
