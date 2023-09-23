 
namespace Skynet.Domain.GuildData
{
    public class LavalinkTrackBot : IEntityBase
    {  
        public int Id { get; set; } 
        //Nullable as a track can be part of a playlist or directly a child of GuildMusicData
        public GuildMusicData? GuildMusicData { get; set; }
        public int?  GuildMusicDataId {get;set;}
        public MusicPlaylist? MusicPlaylist { get; set; }
        public int? MusicPlaylistId { get; set; }
        public string Author { get; set; } 
          public string Title { get; set; }  
         public Uri Uri { get; set; }
         public LavalinkTrackBot()
        { 
        }
      
    }
}
