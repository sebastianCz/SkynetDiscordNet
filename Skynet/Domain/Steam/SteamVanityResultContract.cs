using System.Text.Json.Serialization;

namespace Skynet.Domain.Steam
{
    public class SteamVanityResultContract
    {
        //{"response":{"steamid":"76561198069958454","success":1}}
        [JsonPropertyName("steamid")]
        public string SteamId { get; set; }
        [JsonPropertyName("success")]
        public int Success { get; set; }
        public SteamVanityResultContract() { }
    }
}
