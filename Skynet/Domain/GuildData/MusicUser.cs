namespace Skynet.Domain.GuildData
{
    public class MusicUser: IEntityBase
    {
        public int  Id { get; set; }
        public string UserName { get; set; }
        public DateTime LastUsed { get; set; } 
        public List<MusicSearchTerm> SearchTerms { get; set; } = new List<MusicSearchTerm>(); 
        
    }
}
