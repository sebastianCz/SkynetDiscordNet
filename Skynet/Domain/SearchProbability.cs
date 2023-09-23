using Skynet.Domain.GuildData;

namespace Skynet.Domain
{
    public class SearchProbability
    {
        public int Id { get; set; }
        public GuildMusicData GuildMusicData { get; set; }
        public int GuildMusicDataId { get; set; }
        public int AutoplayGuildPlaylists { get; set; }
        public int AutoPlayUserTerms { get; set; }
        public int AutoplayRandomTerm { get; set; }
        public int AutoPlayRandomPlaylist { get; set; }
        public int AutoPlayDefaultTracks { get; set; }
        public SearchProbability()
        {
            AutoplayGuildPlaylists = 100;
            AutoPlayUserTerms = 1;
            AutoplayRandomTerm = 1;
            AutoPlayRandomPlaylist = 1;
            AutoPlayDefaultTracks = 20;
        }
    }
}
