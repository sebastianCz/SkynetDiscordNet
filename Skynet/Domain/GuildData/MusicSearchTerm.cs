namespace Skynet.Domain.GuildData
{
    public class MusicSearchTerm : IEntityBase
    {
        public int Id { get; set; }
        public DateTime AddedOn { get; set; }
        public string Term { get; set; }
        public GuildMusicData? GuildMusicData { get; set; }
        public int? GuildMusicDataId { get; set; }
        public MusicUser? MusicUser{ get;set;}
        public int? MusicUserId { get; set; }
    }
}
