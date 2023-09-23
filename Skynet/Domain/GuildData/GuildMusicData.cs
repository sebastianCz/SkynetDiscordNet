using DSharpPlus.Lavalink;

namespace Skynet.Domain.GuildData
{
    public class GuildMusicData: IEntityBase
    {
        public int Id { get; set; }
        public string DiscordId { get; set; }
        public bool AutoplayOn { get; set; }
        public List<LavalinkTrackBot> ActivePlaylist{get;set;}
        public List<MusicPlaylist> Playlists { get; set; }
        public List<MusicSearchTerm> SearchTerms { get; set; } 
        public SearchProbability SearchProbability { get; set; }
        public List<SearchEngineResult> SearchEngines { get; set; }

        public GuildMusicData()
        {
               Playlists = new List<MusicPlaylist>();
               SearchTerms = new List<MusicSearchTerm>();
               ActivePlaylist = new List<LavalinkTrackBot>();
            SearchEngines = new List<SearchEngineResult>();
        }
    }
}
