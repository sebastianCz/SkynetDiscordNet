using Skynet.Domain.Enum;

namespace Skynet.Domain
{
    public class GuildMusicReport
    {
        public int SearchesCount { get; set; }
        public LavalinkSearchTypeInt MostCommonLavalinkSearchType { get; set; }
        public SearchType MostCommonSearchPattern { get; set; }
        public SearchInput MostCommonSearchInput { get; set; }
        public string MostCommonSongName { get; set; }
        public string MostCommonSongUri { get; set; }
        public GuildMusicReport()
        {

        }

    }
}
