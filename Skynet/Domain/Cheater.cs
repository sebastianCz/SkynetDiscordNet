namespace Skynet.Domain
{
    public class Cheater
    {
        public int Id { get; set; }
        public string SteamId { get; set; }
        public List<string> Names { get; set; }

        public bool PrivateProfile { get; set; }
        public DateTime Added { get; set; }
        public DateTime LastVerified { get; set; }
    }
}
