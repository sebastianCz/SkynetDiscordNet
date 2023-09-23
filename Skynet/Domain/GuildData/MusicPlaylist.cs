using DSharpPlus.Lavalink;
using Microsoft.VisualBasic;

namespace Skynet.Domain.GuildData
{
    public class MusicPlaylist: IEntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string CreatedBy { get; set; }
        public DateTime DateTime { get; set; }
        public List<LavalinkTrackBot> Playlist { get; set; } = new List<LavalinkTrackBot>();
        public GuildMusicData? GuildMusicData { get; set; }
        public int? GuildMusicDataId { get; set; }
    }
}
