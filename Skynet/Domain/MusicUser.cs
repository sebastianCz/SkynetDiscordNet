namespace Skynet.Domain
{
    public class MusicUser
    {
        public List<MusicSearchTerm> SearchTerms { get; set; }
        public string UserName { get; set; } 
        public DateTime LastUsed { get; set; }
    }
}
